using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Democrachat.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Democrachat.Chat
{
    [Authorize]
    public class ChatHub : Hub
    {
        private IAuthService _authService;
        private ITopicNameService _topicNameService;
        private ActiveUserService _activeUserService;

        public ChatHub(IAuthService authService, ITopicNameService topicNameService,
            ActiveUserService activeUserService)
        {
            _authService = authService;
            _topicNameService = topicNameService;
            _activeUserService = activeUserService;
        }

        public void SendMessage(string topic, string message)
        {
            if (!_topicNameService.IsValidTopic(topic))
                return;
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            Clients.Group(topic).SendAsync("ReceiveMessage", topic, _authService.GetUserById(userId).Username, message);
            _activeUserService.AddUserToTopic(topic, userId);
        }

        public void JoinTopic(string topic)
        {
            if (!_topicNameService.IsValidTopic(topic))
                return;
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            _activeUserService.AddUserToTopic(topic, userId);
            Groups.AddToGroupAsync(Context.ConnectionId, topic);
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            _activeUserService.DisconnectUser(userId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}