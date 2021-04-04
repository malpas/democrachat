using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Inventory
{
    [ApiController]
    [Route("/api/inventory")]
    [Authorize]
    public class InventoryController : ControllerBase
    {
        private IInventoryService _inventoryService;

        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }
        
        [HttpGet]
        public IEnumerable<PublicItem> GetInventory()
        {
            var userId = int.Parse(HttpContext.User.FindFirstValue("Id"));
            return _inventoryService.GetItemsForUserId(userId)
                .Select(PublicItem.FromItem);
        }

        [HttpPost("use")]
        public IActionResult UseInventory([FromBody] UseItemRequest request)
        {
            var userId = int.Parse(HttpContext.User.FindFirstValue("Id"));
            return Ok(_inventoryService.UseItem(userId, request.Uuid));
        }
    }
}