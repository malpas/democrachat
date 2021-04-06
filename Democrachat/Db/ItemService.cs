using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Democrachat.Db.Models;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Democrachat.Db
{
    public class ItemService : IItemService
    {
        private IConfiguration _config;

        public ItemService(IConfiguration config)
        {
            _config = config;
        }
        
        public IEnumerable<Item> GetItemsForUserId(int userId)
        {
            var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            var items = conn.Query<Item>("SELECT * from item WHERE owner_id = @Id", new {Id = userId})
                .Select(item => RetrieveTemplateInfo(item, conn));
            conn.Close();
            return items;
        }

        public Item? GetItemByUuid(Guid uuid)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            var item = conn.QueryFirstOrDefault<Item>("SELECT * from item WHERE public_uuid = @Uuid",
                new {Uuid = uuid});
            return item == null ? null : RetrieveTemplateInfo(item, conn);
        }

        public void DeleteItemByUuid(Guid uuid)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("DELETE FROM item WHERE public_uuid = @Uuid", new {Uuid = uuid});
        }

        public void CreateItem(int userId, int templateId)
        {
            using var conn = new NpgsqlConnection(_config.GetConnectionString("Default"));
            conn.Execute("INSERT INTO item (owner_id, template_id) VALUES (@UserId, @TemplateId)",
                new {UserId = userId, TemplateId = templateId});
        }

        private Item RetrieveTemplateInfo(Item item, IDbConnection conn)
        {
            var (script, name, imageSrc) = conn.QueryFirst<(string, string, string)>("SELECT script, name, image_src FROM item_template WHERE id = @TemplateId",
                new {item.TemplateId});
            return item with {Script = script, Name = name, ImageSrc = imageSrc};
        }
    }
}