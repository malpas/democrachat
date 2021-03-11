using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using Democrachat.Db;
using Democrachat.Log;

namespace Democrachat.Power
{
    public class TopicBidService
    {
        private ITopicService _topicService;
        private IUserService _userService;
        private ILogger _logger;

        public TopicBidService(ITopicService topicService, IUserService userService, ILogger logger)
        {
            _topicService = topicService;
            _userService = userService;
            _logger = logger;
        }

        public Result Bid(int userId, string name, int silver)
        {
            name = name.ToLower();
            var userData = _userService.GetDataById(userId);
            if (userData == null)
            {
                return Result.NO_USER_DATA;
            }
            if (userData.Silver < silver)
            {
                return Result.NOT_ENOUGH_SILVER;
            }
            _topicService.AddBid(userId, name, silver);
            _userService.SubtractSilver(userId, silver);
            var topicSha = SHA512.HashData(Encoding.UTF8.GetBytes(name));
            var topicHex = Convert.ToHexString(topicSha).Substring(13, 5).ToLower();
            _logger.WriteLog($"topicBid user={userData.Username} hash={topicHex} silver={silver}");
            return Result.OK;
        }
        
        public enum Result
        {
            OK,
            NOT_ENOUGH_SILVER,
            NO_USER_DATA
        }
    }
}