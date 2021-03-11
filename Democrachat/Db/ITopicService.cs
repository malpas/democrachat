namespace Democrachat.Db
{
    public interface ITopicService
    {
        void AddBid(int userId, string topicName, int silver);
    }
}