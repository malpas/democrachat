using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using Democrachat.Db;
using Democrachat.Db.Models;
using Democrachat.Kudo;
using Democrachat.Log;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    public class KudoTest
    {
        private Mock<IKudoTableService> _mockKudoTableService;
        private Mock<IItemService> _mockItemService;
        private Mock<IUserService> _mockUserService;
        private KudoService _kudoService;
        private Mock<ILogger> _mockLogger;

        public KudoTest()
        {
            _mockKudoTableService = new Mock<IKudoTableService>();
            _mockItemService = new Mock<IItemService>();
            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger>();
            var sender = new UserData {Id = 1, Username = "sender", LastKudoTime = DateTime.Now - TimeSpan.FromHours(8)};
            var recipient = new UserData {Id = 2, Username = "recipient"};
            _mockUserService.Setup(s => s.GetDataById(1)) .Returns(sender);
            _mockUserService.Setup(s => s.GetDataByUsername("sender")).Returns(sender);
            _mockUserService.Setup(s => s.GetDataByUsername("recipient")).Returns(recipient);

            _kudoService = new KudoService(_mockKudoTableService.Object, _mockItemService.Object,
                _mockUserService.Object, _mockLogger.Object);
        }
        
        [Fact]
        void KudoGivesUserItemFromKudoTable()
        {           
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 100)
            });
            
            _kudoService.GiveKudo(1, "recipient", IPAddress.Any);
            
            _mockItemService.Verify(s => s.CreateItem(2, 1));
        }

        [Fact]
        void KudosAreProperlyWeighted()
        {
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 1),
                new(2, 2),
                new(3, 3)
            });           
            var itemHandouts = new Dictionary<int, int>();
            _mockItemService.Setup(s => s.CreateItem(2, It.IsAny<int>()))
                .Callback((int _, int templateId) =>
                {
                    if (!itemHandouts.ContainsKey(templateId))
                        itemHandouts[templateId] = 0;
                    ++itemHandouts[templateId];
                });
            
            for (var i = 0; i < 20000; ++i)
            {
                _kudoService.GiveKudo(1, "recipient", IPAddress.Any);
            }

            _mockItemService.Verify(s => s.CreateItem(2, 1));
            _mockItemService.Verify(s => s.CreateItem(2, 2));
            _mockItemService.Verify(s => s.CreateItem(2, 3));
            Assert.True(itemHandouts[2] > itemHandouts[1], "Weight of 2 should be more common than weight of 1");
            Assert.True(itemHandouts[3] > itemHandouts[2], "Weight of 3 should be more common than weight of 2");
        }

        [Fact]
        void EqualWeightsGetReceived()
        {
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 1),
                new(2, 1)
            });                
            
            for (var i = 0; i < 20000; ++i)
            {
                _kudoService.GiveKudo(1, "recipient", IPAddress.Loopback);
            }
            
            _mockItemService.Verify(s => s.CreateItem(2, 1));
            _mockItemService.Verify(s => s.CreateItem(2, 2));
        }

        [Fact]
        void KudoRecipientMustExist()
        {    
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 1)
            });         
            
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _kudoService.GiveKudo(1, "nonexistent", IPAddress.Loopback);
            });
            Assert.Contains("user", exception.Message);
        }

        [Fact]
        void CannotGiveKudoToOneself()
        {
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 1)
            });            
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _kudoService.GiveKudo(1, "sender", IPAddress.Loopback);
            });
            Assert.Contains("yourself", exception.Message);
        }

        [Fact]
        void MustNotHaveGivenKudoInLast8Hours()
        {
            _mockUserService.Setup(s => s.GetDataById(1))
                .Returns(new UserData {Id = 1, LastKudoTime = DateTime.Now});      
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 1)
            });         
            
            var exception = Assert.Throws<InvalidOperationException>(() =>
            {
                _kudoService.GiveKudo(1, "recipient", IPAddress.Loopback);
            });
            Assert.Contains("8 hours", exception.Message);
        }

        [Fact]
        void GivingKudoUpdatesKudoTime()
        {
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 1)
            });
            
            _kudoService.GiveKudo(1, "recipient", IPAddress.Loopback);
            _mockUserService.Verify(s => s.UpdateKudoTime(1, It.IsAny<DateTime>()));
        }

        [Fact]
        void KudosAreLogged()
        {
            _mockKudoTableService.Setup(s => s.GetPossibleKudoItems()).Returns(new List<KudoListing>
            {
                new(1, 1)
            });
            
            _kudoService.GiveKudo(1, "recipient", IPAddress.Loopback);
            var hash = SHA256.HashData(IPAddress.Loopback.GetAddressBytes());
            var hashText = BitConverter.ToString(hash).Replace("-", "").Substring(3, 5).ToLower();
            _mockLogger.Verify(l => l.WriteLog($"kudo from=sender to=recipient hash={hashText}"));
        }
    }
}