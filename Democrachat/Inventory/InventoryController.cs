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
        public IActionResult UseItem([FromBody] UseItemRequest request)
        {
            var userId = int.Parse(HttpContext.User.FindFirstValue("Id"));
            var result = _inventoryService.UseItem(userId, request.Uuid);
            if (result.Type == ItemResultType.Error)
            {
                return BadRequest(new {Error = result.Message});
            }
            return Ok(new { Message = result.Message});
        }
    }
}