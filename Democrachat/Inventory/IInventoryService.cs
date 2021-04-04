using System;
using System.Collections.Generic;
using Democrachat.Db.Models;

namespace Democrachat.Inventory
{
    public interface IInventoryService
    {
        public ItemResult UseItem(int userId, Guid itemUuid);
        public IEnumerable<Item> GetItemsForUserId(int userId);
    }
}