using System;
using System.IO;

namespace Democrachat.Log
{
    public class Logger : ILogger
    {
        public string ReadLog()
        {
            try
            {
                return $"democrachat {DateTime.Now.ToUniversalTime()}\n" + File.ReadAllText("log.txt");
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