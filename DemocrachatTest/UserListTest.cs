using System.Collections.Generic;
using System.Security.Claims;
using Democrachat.Auth;
using Democrachat.Chat;
using Democrachat.Db;
using Democrachat.Db.Models;
using Democrachat.Log;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    public class UserListTest
    {
        private ChatHub _hub;
        private ActiveUserService _activeUserService;

        public UserListTest()
        {
            var mockLogger = new Mock<ILogger>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.BatchGetUsernamesByIds(new[] {1}))
                .Returns(new[] {"testuser"});
            mockUserService.Setup(s => s.BatchGetUsernamesByIds(new int[] {}))
                .Returns(new[] {""});
            mockUserService.Setup(s => s.GetDataById(1)).Returns(new UserData {Username = "testuser"});
            var mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(c => c.User!.FindFirst(It.IsAny<string>()))
                .Returns(new Claim("Id", "1"));
            var mockTopicService = new Mock<ITopicNameService>();
            mockTopicService.Setup(s => s.IsValidTopic("general")).Returns(true);
            _activeUserService = new ActiveUserService(mockUserService.Object);
            var mockGroups = new Mock<IGroupManager>();
            var mockClients = new Mock<IHubCallerClients>();
            mockClients.Setup(c =>
                    c.Group(It.IsAny<string>()).SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]?>(),
                        default))
                .Callback(() => { });

            _hub = new ChatHub(mockUserService.Object, mockTopicService.Object, _activeUserService, mockLogger.Object,
                new ChatSpamService())
            {
                Context = mockContext.Object,
                Groups = mockGroups.Object,
                Clients = mockClients.Object
            };
        }
        
        [Fact]
        public void AddActiveUserTopic()
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic("general");
            Assert.Contains("testuser", _activeUserService.GetUsersInTopic("general"));
        }

        [Fact]
        public void UserDisconnection()
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic("general");
            _hub.OnDisconnectedAsync(null);
            Assert.DoesNotContain("testuser", _activeUserService.GetUsersInTopic("general"));
        }

        [Fact]
        public void GetUserList()
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic("general");
            var controller = new ActiveUserController(_activeUserService);
            var result = controller.GetUsersInTopic("general") as OkObjectResult;
            Assert.Equal(new [] {"testuser"}, result?.Value);
        }

        [Fact]
        public void GetEmptyUserList()
        {
            var controller = new ActiveUserController(_activeUserService);
            var result = controller.GetUsersInTopic("nothing") as OkObjectResult;
            var list = result?.Value as IEnumerable<string>;
            Assert.Empty(list!);
        }

        [Fact]
        public void SendingMessageMarksActivity()
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic("general");
            _hub.OnDisconnectedAsync(null);
            _hub.SendMessage("general", "Blah");
            var controller = new ActiveUserController(_activeUserService);
            var result = controller.GetUsersInTopic("general") as OkObjectResult;
            Assert.Equal(new [] {"testuser"}, result?.Value);
        }
    }
}