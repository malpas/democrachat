using System;
using System.IO;
using System.Linq;
using Democrachat.Auth;

namespace Democrachat.Log
{
    public class Logger : ILogger
    {
        private IAuthService _authService;

        public Logger(IAuthService authService)
        {
            _authService = authService;
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
    }
}