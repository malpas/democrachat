namespace Democrachat.Chat
{
    public interface IChatSpamService
    {
        bool CanSend(int userId);
        void MarkSend(int userId);
    }
}