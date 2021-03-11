using System;
using System.Timers;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Democrachat.Rewards
{
    /// <summary>
    /// By owning gold, players should automatically accrue silver based on a timer.
    /// The silver itself is used for things like bidding on new topics and muting other
    /// users.
    /// </summary>
    public class RewardService
    {
        private IConfiguration _config;

        public RewardService(IConfiguration config, ILoggerFactory logFactory)
        {
            _config = config;
            var timer = new Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            var logger = logFactory.CreateLogger<RewardService>();
            timer.Elapsed += (_, _) =>
            {
                logger.LogInformation("Handing out silver");
                GiveOutInterest();
            };
            timer.Start();
        }

        private void GiveOutInterest()
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("UPDATE account SET silver = silver + gold");
        }
    }
}