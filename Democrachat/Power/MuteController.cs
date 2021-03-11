using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Chat
{
    [ApiController]
    [Authorize]
    [Route("/api/mute")]
    public class MuteController : ControllerBase
    {
        private MuteService _muteService;

        public MuteController(MuteService muteService)
        {
            _muteService = muteService;
        }
        
        [HttpPost]
        public IActionResult Mute([FromBody] MuteRequest request)
        {
            var callerId = int.Parse(User.FindFirstValue("Id"));
            var result = _muteService.TryAddMuteTime(request.Username, request.Silver, callerId);
            return result switch
            {
                MuteService.MuteRequestResult.INVALID_USERNAME => BadRequest($"Invalid username {request.Username}"),
                MuteService.MuteRequestResult.NOT_ENOUGH_SILVER => BadRequest("Not enough silver"),
                _ => Ok()
            };
        }
    }
}