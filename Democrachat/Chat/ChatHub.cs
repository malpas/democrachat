using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Timers;
using Castle.Core.Internal;
using Democrachat.Db;
using Democrachat.Log;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Democrachat.Chat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private IUserService _userService;
        private ITopicNameService _topicNameService;
        private ActiveUserService _activeUserService;
        private ILogger _logger;
        private IChatSpamService _spamService;


        public ChatHub(IUserService userService, ITopicNameService topicNameService,
            ActiveUserService activeUserService, ILogger logger, IChatSpamService spamService)
        {
            _userService = userService;
            _topicNameService = topicNameService;
            _activeUserService = activeUserService;
            _logger = logger;
            _spamService = spamService;
        }

        public void SendMessage(string topic, string message)
        {
            message = message.Trim();
            if (message.Length > 400)
            {
                message = message.Substring(0, 400);
            }
            if (message.IsNullOrEmpty())
                return;
            if (!_topicNameService.IsValidTopic(topic))
                return;
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            var userData = _userService.GetDataById(userId);
            if (DateTime.Now <= userData.MutedUntil)
            {
                var secondsLeft = Math.Round((userData.MutedUntil - DateTime.Now).TotalSeconds + 1);
                Clients.Caller.SendAsync("ReceiveMessage", topic, "cc",
                    $"You're muted. Wait {secondsLeft} seconds.");
                return;
            }

            if (!_spamService.CanSend(userId))
            {                
                Clients.Caller.SendAsync("ReceiveMessage", topic, "cc",
                    $"Uh oh, you've got 0 MessageCoins!");
                return;
            }
            _spamService.MarkSend(userId);
            Clients.Group(topic).SendAsync("ReceiveMessage", topic, _userService.GetDataById(userId)?.Username, message);
            _activeUserService.AddUserToTopic(topic, userId);
            _logger.LogChatMessage(userId, topic, message);
        }

        public void IndicateTyping(string topic)
        {
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            Clients.Group(topic).SendAsync("UserTyping", topic, _userService.GetDataById(userId)?.Username);
        }

        public void JoinTopic(string topic)
        {
            if (!_topicNameService.IsValidTopic(topic))
                return;
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            _activeUserService.AddUserToTopic(topic, userId);
            Groups.AddToGroupAsync(Context.ConnectionId, topic);
        }

        public void LeaveTopic(string topic)
        {
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            _activeUserService.RemoveUserFromTopic(topic, userId);
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            _activeUserService.DisconnectUser(userId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}