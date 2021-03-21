using System;
using System.Collections.Generic;
using Democrachat.Auth.Models;
using Democrachat.Db.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Democrachat.Auth
{
    public class DbAuthService : IAuthService
    {
        private IConfiguration _config;

        public DbAuthService(IConfiguration config)
        {
            _config = config;
        }

        public UserData? AttemptLogin(string username, string password)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            var hash = conn.QueryFirstOrDefault<string>("SELECT hash from account where lower(username) = lower(@Username)",
                new {Username = username});
            if (hash != null && !BCrypt.Net.BCrypt.Verify(password, hash))
            {
                return null;
            }
            var userData = conn.QueryFirstOrDefault<UserData>("SELECT * from account where lower(username) = lower(@Username)",
                new {Username = username});
            return userData;
        }

        public RegistrationResult RegisterUser()
        {
            var random = new Random();
            var username = $"user{random.Next(1, 100000)}";
            
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            var id = conn.QueryFirst<int>("INSERT INTO account (username) VALUES (@Username) RETURNING id",
                new { Username = username });
            return new RegistrationResult(id);
        }

        public UserData? GetUserById(int id)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.QueryFirstOrDefault<UserData>("SELECT * FROM account WHERE id = @Id",
                new { Id = id });
        }

        public void FinalizeNewUser(int id, string username, string password)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("UPDATE account SET username = @Username, hash = @Hash WHERE id = @Id",
                new { Id = id, Username = username, Hash = BCrypt.Net.BCrypt.HashPassword(password) });
        }

        public bool IsUsernameTaken(string username)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.QueryFirst<bool>("SELECT EXISTS (SELECT * FROM account WHERE lower(username) = lower(@Username))",
                new {Username = username});
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

        public IEnumerable<UserData> GetOrderedUsersWithWealth()
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.Query<UserData>("SELECT * FROM account WHERE gold > 0 OR silver > 0 ORDER BY gold DESC, silver DESC");
        }
    }
}