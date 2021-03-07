using System.Security.Claims;
using System.Threading;
using Democrachat.Auth;
using Democrachat.Auth.Models;
using Democrachat.Chat;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    /// <summary>
    /// Tests for chat functionality
    /// </summary>
    public class MessageTest
    {
        private ChatHub _hub;
        private Mock<IHubCallerClients> _mockClients;

        public MessageTest()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new [] {new Claim("Id", "10")}));
            var mockContext = new Mock<HubCallerContext>();
            mockContext.Setup(c => c.User).Returns(user);
            _mockClients = new Mock<IHubCallerClients>();
            _mockClients.Setup(c =>
                    c.Group(It.IsAny<string>()).SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]?>(),
                        It.IsAny<CancellationToken>()))
                .Callback(() => { });
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.GetUserById(10))
                .Returns(new UserData {Username = "john"});
            var mockTopicValidator = new Mock<ITopicNameService>();
            mockTopicValidator.Setup(s => s.IsValidTopic("abc")).Returns(true);
            var activeUserService = new ActiveUserService(mockAuthService.Object);
            _hub = new ChatHub(mockAuthService.Object, mockTopicValidator.Object, activeUserService)
            {
                Context = mockContext.Object, 
                Clients = _mockClients.Object
            };
        }
        
        [Fact]
        public void ClientsReceiveMessages()
        {
            _hub.SendMessage("abc", "test");
            _mockClients.Verify(c => c.Group("abc").SendCoreAsync("ReceiveMessage", new object?[] {"abc", "john", "test"}, default),
                Times.Once);
        }

        [Fact]
        public void OnlyAllowValidTopics()
        {
            _hub.SendMessage("notvalid", "test");
            _mockClients.Verify(c => c.Group("notvalid").SendCoreAsync("ReceiveMessage", It.IsAny<object[]?>(), default),
                Times.Never);
        }
    }
}