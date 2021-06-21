using System.Collections.Generic;
using System.Security.Claims;
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
        private Mock<IHubCallerClients> _mockClients;

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
            mockTopicService.Setup(s => s.GetTopics()).Returns(new List<string> { "general", "gaming"});
            mockTopicService.Setup(s => s.IsValidTopic("general")).Returns(true);
            mockTopicService.Setup(s => s.IsValidTopic("gaming")).Returns(true);
            _activeUserService = new ActiveUserService(mockUserService.Object);
            var mockGroups = new Mock<IGroupManager>();
            _mockClients = new Mock<IHubCallerClients>();
            _mockClients.Setup(c =>
                    c.Group(It.IsAny<string>()).SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]?>(),
                        default))
                .Callback(() => { });
            
            _mockClients.Setup(c =>
                    c.All.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]?>(),
                        default))
                .Callback(() => { });

            _hub = new ChatHub(mockUserService.Object, mockTopicService.Object, _activeUserService, mockLogger.Object,
                new ChatSpamService())
            {
                Context = mockContext.Object,
                Groups = mockGroups.Object,
                Clients = _mockClients.Object
            };
        }
        
        [Theory]
        [InlineData("general")]
        [InlineData("gaming")]
        public void AddActiveUserTopic(string topic)
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic(topic);
            Assert.Contains("testuser", _activeUserService.GetUsersInTopic(topic));
        }

        [Theory]
        [InlineData("general")]
        [InlineData("gaming")]
        public void JoiningTopicSendsHubAlert(string topic)
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic(topic);
            _mockClients.Verify(c => 
                c.All.SendCoreAsync("UserJoined", new object?[]{ topic, "testuser"}, default));
        }
        
        [Theory]
        [InlineData("general")]
        [InlineData("gaming")]
        public void LeavingTopicSendsHubAlert(string topic)
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic(topic);
            _hub.LeaveTopic(topic);
            _mockClients.Verify(c => 
                c.All.SendCoreAsync("UserLeft", new object?[]{ topic, "testuser"}, default));
        }

        [Fact]
        public void UserDisconnection()
        {
            _hub.OnConnectedAsync();
            _hub.JoinTopic("general");
            _hub.OnDisconnectedAsync(null);
            Assert.DoesNotContain("testuser", _activeUserService.GetUsersInTopic("general"));
            _mockClients.Verify(c => 
                c.All.SendCoreAsync("UserLeft", new object?[]{ "general", "testuser"}, default));
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