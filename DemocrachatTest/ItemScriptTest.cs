using System;
using Democrachat.Db;
using Democrachat.Db.Models;
using Democrachat.Inventory;
using Moq;
using Xunit;

namespace DemocrachatTest
{
    /// <summary>
    /// Test the behaviour of using items
    /// </summary>
    public class ItemScriptTest
    {
        private Mock<IUserService> _mockUserService;
        private Mock<IItemService> _mockItemService;
        private InventoryService _inventoryService;

        public ItemScriptTest()
        {
            _mockUserService = new Mock<IUserService>();
            _mockItemService = new Mock<IItemService>();
            _mockUserService.Setup(s => s.GetDataById(1)).Returns(new UserData { Id = 1, Username = "tester"});
            _inventoryService = new InventoryService(_mockUserService.Object, _mockItemService.Object);
        }
        
        [Fact]
        void CanAddSilver()
        {
            var item = new Item {Name = "Silver Adder", Script = "AddSilver(10)", OwnerId = 1};
            _mockItemService.Setup(s => s.GetItemByUuid(Guid.Empty)).Returns(item);

            var result = _inventoryService.UseItem(1, Guid.Empty);
            
            Assert.Equal(new ItemResult(ItemResultType.Success, null), result);
            _mockUserService.Verify(s => s.AddSilver(1, 10));
        }

        [Fact]
        void CanSendMessage()
        {
            var item = new Item {Name = "Message Sender", Script = "Message = 'Success!'", OwnerId = 1};
            _mockItemService.Setup(s => s.GetItemByUuid(Guid.Empty)).Returns(item);
            
            var result = _inventoryService.UseItem(1, Guid.Empty);
            
            Assert.Equal(new ItemResult(ItemResultType.Success, "Success!"), result);
        }

        [Fact]
        void CanChainActions()
        {
            var item = new Item {Name = "Silver Adder", Script = "AddSilver(10); AddSilver(10)", OwnerId = 1};
            _mockItemService.Setup(s => s.GetItemByUuid(Guid.Empty)).Returns(item);

            var result = _inventoryService.UseItem(1, Guid.Empty);
            
            Assert.Equal(new ItemResult(ItemResultType.Success, null), result);
            _mockUserService.Verify(s => s.AddSilver(1, 10), Times.Exactly(2));
        }

        [Fact]
        void UsingItemDeletesItem()
        {
            var item = new Item {Name = "Silver Adder", Script = "", OwnerId = 1};
            _mockItemService.Setup(s => s.GetItemByUuid(Guid.Empty)).Returns(item);

            var result = _inventoryService.UseItem(1, Guid.Empty);
            
            _mockItemService.Verify(s => s.DeleteItemByUuid(Guid.Empty));
        }
    }
}