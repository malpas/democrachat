using System;
using Democrachat.Auth.Models;
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
    }
}