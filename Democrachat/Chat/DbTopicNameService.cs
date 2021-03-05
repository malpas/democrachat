using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Democrachat.Chat
{
    public class DbTopicNameService : ITopicNameService
    {
        private IConfiguration _config;

        public DbTopicNameService(IConfiguration config)
        {
            _config = config;
        }
        
        public bool IsValidTopic(string name)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            return conn.QueryFirst<bool>("SELECT EXISTS (SELECT * FROM topic WHERE name = @Name)",
                new { Name = name });
        }
    }
}