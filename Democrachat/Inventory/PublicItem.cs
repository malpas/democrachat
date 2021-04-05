using System;
using Democrachat.Db.Models;

namespace Democrachat.Inventory
{
    public record PublicItem(string Name, Guid Uuid, string? ImageSrc)
    {
        public static PublicItem FromItem(Item item)
        {
            return new(item.Name, item.PublicUuid, item.ImageSrc);
        }
    }
}