using System;
using System.Security.Claims;
using System.Threading;
using Democrachat.Auth;
using Democrachat.Auth.Models;
using Democrachat.Chat;
using Democrachat.Db.Models;
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
        private Mock<HubCallerContext> _mockContext;
        
        private ClaimsPrincipal johnPrincipal = 
            new(new ClaimsIdentity(new [] {new Claim("Id", "10")}));
        private ClaimsPrincipal mutedGuyPrincipal = 
            new(new ClaimsIdentity(new [] {new Claim("Id", "5")}));

        public MessageTest()
        {
            _mockContext = new Mock<HubCallerContext>();
            _mockClients = new Mock<IHubCallerClients>();
            _mockClients.Setup(c =>
                    c.Group(It.IsAny<string>()).SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]?>(),
                        It.IsAny<CancellationToken>()))
                .Callback(() => { });
            _mockClients.Setup(c =>
                    c.Caller.SendCoreAsync(It.IsAny<string>(), It.IsAny<object?[]?>(),
                        It.IsAny<CancellationToken>()))
                .Callback(() => { });
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(s => s.GetUserById(10))
                .Returns(new UserData {Username = "john"});
            mockAuthService.Setup(s => s.GetUserById(5))
                .Returns(new UserData {Username = "mutedguy", Id = 5, MutedUntil = DateTime.Now + TimeSpan.FromMinutes(1)});
            var mockTopicValidator = new Mock<ITopicNameService>();
            mockTopicValidator.Setup(s => s.IsValidTopic("abc")).Returns(true);
            var activeUserService = new ActiveUserService(mockAuthService.Object);
            _hub = new ChatHub(mockAuthService.Object, mockTopicValidator.Object, activeUserService)
            {
                Context = _mockContext.Object, 
                Clients = _mockClients.Object
            };
        }

        [Fact]
        public void ClientsReceiveMessages()
        {
            _mockContext.Setup(c => c.User).Returns(johnPrincipal);
            _hub.SendMessage("abc", "test");
            _mockClients.Verify(c => c.Group("abc").SendCoreAsync("ReceiveMessage", new object?[] {"abc", "john", "test"}, default),
                Times.Once);
        }

        [Fact]
        public void BlankMessagesNotSent()
        {
            _mockContext.Setup(c => c.User).Returns(johnPrincipal);
            _hub.SendMessage("abc", "");
            _mockClients.Verify(c => c.Group("abc").SendCoreAsync("ReceiveMessage", It.IsAny<object?[]>(), default),
                Times.Never);
        }

        [Fact]
        public void OnlyAllowValidTopics()
        {
            _mockContext.Setup(c => c.User).Returns(johnPrincipal);
            _hub.SendMessage("notvalid", "test");
            _mockClients.Verify(c => c.Group("notvalid").SendCoreAsync("ReceiveMessage", It.IsAny<object[]?>(), default),
                Times.Never);
        }

        [Fact]
        public void NoSendingWhenMuted()
        {
            _mockContext.Setup(c => c.User).Returns(mutedGuyPrincipal);
            _hub.SendMessage("abc", "test");
            _mockClients.Verify(c => c.Group("abc").SendCoreAsync("ReceiveMessage", It.IsAny<object[]?>(), default),
                Times.Never);
        }
    }
}