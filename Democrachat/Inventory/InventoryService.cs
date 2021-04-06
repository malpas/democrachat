using System;
using System.Collections.Generic;
using Democrachat.Db;
using Democrachat.Db.Models;
using IronPython.Hosting;

namespace Democrachat.Inventory
{
    public class InventoryService : IInventoryService
    {
        private IUserService _userService;
        private IItemService _itemService;

        public InventoryService(IUserService userService, IItemService itemService)
        {
            _userService = userService;
            _itemService = itemService;
        }
        
        public ItemResult UseItem(int userId, Guid itemUuid)
        {
            var item = _itemService.GetItemByUuid(itemUuid);
            if (item == null)
            {
                return new ItemResult(ItemResultType.Error, "Item does not exist");
            }

            if (item.OwnerId != userId)
            {
                return new ItemResult(ItemResultType.Error, "That's not your item");
            }
            var engine = Python.CreateEngine();
            dynamic scope = engine.CreateScope();
            scope.User = _userService.GetDataById(userId)!;
            scope.AddSilver = new Action<int>(amount => _userService.AddSilver(userId, amount));
            scope.SubtractSilver = new Action<int>(amount => _userService.SubtractSilver(userId, amount));
            scope.AddGold = new Action<int>(amount => _userService.AddGold(userId, amount));
            scope.Message = "";
            engine.CreateScriptSourceFromString(item.Script).Execute(scope);
            _itemService.DeleteItemByUuid(itemUuid);
            return new ItemResult(ItemResultType.Success, scope.Message != "" ? scope.Message : null);
        }

        public IEnumerable<Item> GetItemsForUserId(int userId)
        {
            return _itemService.GetItemsForUserId(userId);
        }
    }
}