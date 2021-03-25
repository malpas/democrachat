namespace Democrachat.Log
{
    public interface ILogger
    {
        string ReadLog();
        void WriteLog(string message);
        void LogChatMessage(int userId, string topic, string text);
    }
}