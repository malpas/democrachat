using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using Democrachat.Db;

namespace Democrachat.Chat
{
    public class ActiveUserService
    {
        private Dictionary<string, HashSet<int>> _topicUserIds = new();
        private IUserService _userService;

        public ActiveUserService(IUserService userService)
        {
            _userService = userService;
        }

        public IEnumerable<string> GetUsersInTopic(string topic)
        {
            if (!_topicUserIds.ContainsKey(topic) || _topicUserIds[topic].IsNullOrEmpty())
                return new List<string>();
            var userIds = _topicUserIds[topic];
            return _userService.BatchGetUsernamesByIds(userIds);
        }

        public void AddUserToTopic(string topic, int userId)
        {
            if (!_topicUserIds.ContainsKey(topic))
                _topicUserIds[topic] = new HashSet<int>();
            _topicUserIds[topic].Add(userId);
        }

        public void DisconnectUser(int userId)
        {
            foreach (var (topic, ids) in _topicUserIds)
            {
                RemoveUserFromTopic(topic, userId);
            }
        }

        public void RemoveUserFromTopic(string topic, int userId)
        {
            _topicUserIds[topic] = _topicUserIds[topic].Where(id => id != userId).ToHashSet();
        }
    }
}