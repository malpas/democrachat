using System;
using System.Collections.Generic;
using Democrachat.Db.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Democrachat.Db
{
    public class UserService : IUserService
    {
        private IConfiguration _config;

        public UserService(IConfiguration config)
        {
            _config = config;
        }
        
        public UserData? GetDataById(int id)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.QueryFirstOrDefault<UserData>("SELECT * FROM account WHERE id = @Id", new {ID = id});
        }

        public UserData? GetDataByUsername(string username)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.QueryFirstOrDefault<UserData>("SELECT * FROM account WHERE username = @Username", 
                new {Username = username});
        }

        public void AddMuteTime(int userId, TimeSpan time)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            var timeString = $@"'{time.TotalSeconds} seconds'";
            conn.Execute($"UPDATE account SET muted_until = greatest(now() + {timeString}, muted_until + {timeString}) WHERE id = @Id",
                new { Id = userId });
        }

        public void SubtractSilver(int userId, int amount)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("UPDATE account SET silver = silver - @Amount WHERE id = @Id",
                new { Amount = amount, Id = userId });
        }

        public void AddSilver(int userId, int amount)
        {            
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("UPDATE account SET silver = silver + @Amount WHERE id = @Id",
                new { Amount = amount, Id = userId });
        }

        public void AddGold(int userId, int amount)
        {           
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("UPDATE account SET gold = gold + @Amount WHERE id = @Id",
                new { Amount = amount, Id = userId });
        }

        public void UpdateKudoTime(int userId, DateTime time)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("UPDATE account SET last_kudo_time = @Time", new {Time = time});
        }

        public void FinalizeNewUser(int id, string username, string password)
        {
            
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("UPDATE account SET username = @Username, hash = @Hash WHERE id = @Id",
                new { Id = id, Username = username, Hash = BCrypt.Net.BCrypt.HashPassword(password) });
        }

        public IEnumerable<UserData> GetOrderedUsersByWealth()
        {
            
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.Query<UserData>("SELECT * FROM account WHERE gold > 0 OR silver > 0 ORDER BY username");
        }
        
        public IEnumerable<string> BatchGetUsernamesByIds(IEnumerable<int> ids)
        {
            var query = "";
            var idList = ids.AsList();
            for (int i = 0; i < idList.Count; ++i)
            {
                query += $"SELECT username FROM account WHERE id = {idList[i]} ";
                if (i < idList.Count - 1)
                {
                    query += " UNION ";
                }
            }
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.Query<string>(query);
        }

        public int RegisterUser()
        {
            
            var random = new Random();
            var username = $"user{random.Next(1, 100000)}";
            
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            var id = conn.QueryFirst<int>("INSERT INTO account (username) VALUES (@Username) RETURNING id",
                new { Username = username });
            return id;
        }
    }
}