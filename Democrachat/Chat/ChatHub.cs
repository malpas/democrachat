using System.Security.Claims;
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

        public ChatHub(IAuthService authService, ITopicNameService topicNameService)
        {
            _authService = authService;
            _topicNameService = topicNameService;
        }

        public void SendMessage(string topic, string message)
        {
            if (!_topicNameService.IsValidTopic(topic))
                return;
            var userId = int.Parse(Context.User.FindFirstValue("Id"));
            Clients.Group(topic).SendAsync("ReceiveMessage", topic, _authService.GetUserById(userId).Username, message);
        }
    }
}