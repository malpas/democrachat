using System;
using System.IO;
using System.Linq;
using Democrachat.Auth;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Democrachat.Log
{
    public class Logger : ILogger
    {
        private IAuthService _authService;
        private IConfiguration _config;

        public Logger(IAuthService authService, IConfiguration config)
        {
            _authService = authService;
            _config = config;
        }
        
        public string ReadLog()
        {
            var usersByWealth = _authService.GetOrderedUsersWithWealth();
            var userSection = "username,gold,silver\n" +
                              string.Join("\n", usersByWealth.Select(data => $"{data.Username},{data.Gold},{data.Silver}"));
            try
            {
                return $"democrachat {DateTime.Now.ToUniversalTime()}\n" 
                       + File.ReadAllText("log.txt")
                       + "---\n"
                       + userSection;
            }
            catch
            {
                return "{no_log_data}";
            }
        }

        public void WriteLog(string message)
        {
            File.AppendAllText("log.txt", message + "\n");
        }

        public void LogChatMessage(int userId, string topic, string text)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("INSERT INTO chat_message (user_id, topic, text) VALUES (@UserId, @Topic, @Text)",
                new {UserId = userId, Topic = topic, Text = text});
        }
    }
}