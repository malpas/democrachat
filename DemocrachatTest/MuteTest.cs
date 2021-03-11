using System;
using System.Security.Claims;
using Democrachat.Auth.Models;
using Democrachat.Chat;
using Democrachat.Db;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    public class MuteTest
    {
        [Fact]
        public void MuteAddsMuteTime()
        {
            var mockUserService = new Mock<IUserService>();
            var muteService = new MuteService(mockUserService.Object);
            var controller = new MuteController(muteService);

            mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Username = "caller", Id = 1, Silver = 20});
            mockUserService.Setup(s => s.GetDataByUsername("john"))
                .Returns(new UserData {Username = "john", Id = 2});

            var claims = new Claim[] { new("Id", "1") };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
                {User = new ClaimsPrincipal(new ClaimsIdentity(claims))};
            
            controller.Mute(new MuteRequest { Username = "john", Silver = 20});
            
            mockUserService.Verify(s => s.AddMuteTime(2, TimeSpan.FromSeconds(20 * 5)));
        }

        [Fact]
        public void CanOnlyMuteValidTargets()
        {
            var mockUserService = new Mock<IUserService>();
            var muteService = new MuteService(mockUserService.Object);
            var controller = new MuteController(muteService);
            
            var claims = new Claim[] { new("Id", "1") };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
                {User = new ClaimsPrincipal(new ClaimsIdentity(claims))};
            
            controller.Mute(new MuteRequest { Username = "unknownuser", Silver = 20});
            
            mockUserService.Verify(s => s.AddMuteTime(It.IsAny<int>(), It.IsAny<TimeSpan>()),
                Times.Never);
        }

        [Fact]
        public void RequiresEnoughSilver()
        {
            var mockUserService = new Mock<IUserService>();
            var muteService = new MuteService(mockUserService.Object);
            var controller = new MuteController(muteService);

            mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Username = "caller", Id = 1, Silver = 19});
            mockUserService.Setup(s => s.GetDataByUsername("john"))
                .Returns(new UserData {Username = "john", Id = 2});

            var claims = new Claim[] { new("Id", "1") };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
                {User = new ClaimsPrincipal(new ClaimsIdentity(claims))};
            
            controller.Mute(new MuteRequest { Username = "john", Silver = 20});
            
            mockUserService.Verify(s => s.AddMuteTime(2, TimeSpan.FromSeconds(20 * 5)), 
                Times.Never);
        }

        [Fact]
        public void TakesSilverWhenSuccessful()
        {
            var mockUserService = new Mock<IUserService>();
            var muteService = new MuteService(mockUserService.Object);
            var controller = new MuteController(muteService);

            mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Username = "caller", Id = 1, Silver = 20});
            mockUserService.Setup(s => s.GetDataByUsername("john"))
                .Returns(new UserData {Username = "john", Id = 2});

            var claims = new Claim[] { new("Id", "1") };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
                {User = new ClaimsPrincipal(new ClaimsIdentity(claims))};
            
            controller.Mute(new MuteRequest { Username = "john", Silver = 20});
            
            mockUserService.Verify(s => s.SubtractSilver(1, 20));
        }

        [Fact]
        public void DoesNotTakeSilverWhenUnsuccessful()
        {
            var mockUserService = new Mock<IUserService>();
            var muteService = new MuteService(mockUserService.Object);
            var controller = new MuteController(muteService);

            mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Username = "caller", Id = 1, Silver = 19});
            mockUserService.Setup(s => s.GetDataByUsername("john"))
                .Returns(new UserData {Username = "john", Id = 2});

            var claims = new Claim[] { new("Id", "1") };
            controller.ControllerContext.HttpContext = new DefaultHttpContext
                {User = new ClaimsPrincipal(new ClaimsIdentity(claims))};
            
            controller.Mute(new MuteRequest { Username = "john", Silver = 20});
            
            mockUserService.Verify(s => s.SubtractSilver(1, 20),
                Times.Never);
        }
    }
}