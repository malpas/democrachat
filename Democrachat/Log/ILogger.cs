namespace Democrachat.Log
{
    public interface ILogger
    {
        string ReadLog();
        void WriteLog(string message);
    }
}