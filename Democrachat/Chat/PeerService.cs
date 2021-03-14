using System;
using System.Collections.Generic;
using System.Linq;
using Democrachat.Db;

namespace Democrachat.Chat
{
    public class PeerService
    {
        private Dictionary<string, Registration> _registrations = new();
        private IUserService _userService;

        public PeerService(IUserService userService)
        {
            _userService = userService;
        }
        
        public object? GetPeerData(string peerId)
        {
            if (!_registrations.ContainsKey(peerId))
                return null;
            if (_registrations[peerId].RegisteredAt >= DateTime.Now - TimeSpan.FromHours(1))
                return _registrations.GetValueOrDefault(peerId);
            _registrations.Remove(peerId);
            return null;
        }

        public void RegisterMetaData(int userId, string peerId)
        {
            var data = _userService.GetDataById(userId);
            if (data == null)
            {
                throw new InvalidOperationException("Could not get user data");
            }

            _registrations = _registrations.Where(p => p.Value.Username != data.Username)
                .ToDictionary(i => i.Key, i => i.Value);
            _registrations[peerId] = new Registration(DateTime.Now, data.Username);
        }

        public string? GetPeerId(string username)
        {
            var registration = _registrations
                .Where(keyValuePair => keyValuePair.Value.Username == username)
                .Select(p => p.Key)
                .FirstOrDefault();
            return registration;
        }

        private record Registration(DateTime RegisteredAt, string Username);
    }
}