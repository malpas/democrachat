using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Democrachat.Db
{
    public class TopicService : ITopicService
    {
        private IConfiguration _config;

        public TopicService(IConfiguration config)
        {
            _config = config;
        }
        
        public void AddBid(int userId, string topicName, int silver)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("INSERT INTO topic_bid (user_id, name, silver) VALUES (@UserId, @Name, @Silver)",
                new {UserId = userId, Name = topicName, Silver = silver});
        }
    }
}