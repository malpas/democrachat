using System;
using System.Collections.Generic;
using Democrachat.Db.Models;

namespace Democrachat.Db
{
    public interface IItemService
    {
        IEnumerable<Item> GetItemsForUserId(int userId);
        Item? GetItemByUuid(Guid uuid);
        void DeleteItemByUuid(Guid uuid);
    }
}