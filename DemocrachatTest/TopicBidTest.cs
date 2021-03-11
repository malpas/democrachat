using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Unicode;
using Democrachat.Db;
using Democrachat.Db.Models;
using Democrachat.Log;
using Democrachat.Power;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    public class TopicBidTest
    {
        [Fact]
        public void CanBidForTopic()
        {
            var mockTopicService = new Mock<ITopicService>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Silver = 100, Username = "user2342"});
            
            var mockLogger = new Mock<ILogger>();
            var bidService = new TopicBidService(mockTopicService.Object, mockUserService.Object, mockLogger.Object);
            var bidController = new TopicBidController(bidService);
            bidController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim("Id", "1")}))
            };

            bidController.Bid(new TopicBidRequest {Name = "architecture", Silver = 100});
            
            mockTopicService.Verify(s => s.AddBid(1, "architecture", 100));
            var topicSsh = SHA512.HashData(Encoding.UTF8.GetBytes("architecture"));
            var topicHex = Convert.ToHexString(topicSsh).Substring(13, 5).ToLower();
            mockLogger.Verify(l => l.WriteLog($"topicBid user=user2342 hash={topicHex} silver=100"));
        }

        [Fact]
        public void SuccessfulBidRemovesSilver()
        {
            var mockTopicService = new Mock<ITopicService>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Silver = 5, Username = "user2342"});
            
            var mockLogger = new Mock<ILogger>();
            var bidService = new TopicBidService(mockTopicService.Object, mockUserService.Object, mockLogger.Object);
            var bidController = new TopicBidController(bidService);
            bidController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim("Id", "1")}))
            };

            bidController.Bid(new TopicBidRequest {Name = "architecture", Silver = 5});
            
            mockUserService.Verify(s => s.SubtractSilver(1, 5));
        }

        [Fact]
        public void NeedEnoughSilverToBid()
        {
            var mockTopicService = new Mock<ITopicService>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetDataById(1)).Returns(new UserData {Silver = 99});
            var mockLogger = new Mock<ILogger>();
            var bidService = new TopicBidService(mockTopicService.Object, mockUserService.Object, mockLogger.Object);
            var bidController = new TopicBidController(bidService);
            bidController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim("Id", "1")}))
            };

            bidController.Bid(new TopicBidRequest {Name = "architecture", Silver = 100});
            
            mockTopicService.Verify(s => s.AddBid(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public void BidsAreNotCaseSensitive()
        {
            var mockTopicService = new Mock<ITopicService>();
            var mockUserService = new Mock<IUserService>();
            mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Silver = 100, Username = "user2342"});
            
            var mockLogger = new Mock<ILogger>();
            var bidService = new TopicBidService(mockTopicService.Object, mockUserService.Object, mockLogger.Object);
            var bidController = new TopicBidController(bidService);
            bidController.ControllerContext.HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[] {new Claim("Id", "1")}))
            };

            bidController.Bid(new TopicBidRequest {Name = "ARCHItecture", Silver = 100});
            
            mockTopicService.Verify(s => s.AddBid(1, "architecture", 100));
        }
    }
}