using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Chat
{
    [Authorize]
    [ApiController]
    [Route("/api/peer")]
    public class PeerController : ControllerBase
    {
        private PeerService _peerService;

        public PeerController(PeerService peerService)
        {
            _peerService = peerService;
        }

        [HttpPost("{peerId}")]
        public IActionResult RegisterPeerId(string peerId)
        {
            var userId = int.Parse(User.FindFirstValue("Id"));
            _peerService.RegisterMetaData(userId, peerId);
            return Ok();
        }

        [HttpGet("{peerId}")]
        public IActionResult GetPeerId(string peerId)
        {
            var data = _peerService.GetPeerData(peerId);
            return data == null ? BadRequest("Invalid id") : Ok(data);
        }

        [HttpGet("user/{username}")]
        public IActionResult GetPeerIdFromUsername(string username)
        {
            var peerId = _peerService.GetPeerId(username);
            return peerId == null ? BadRequest("User not available") : Ok(new {Id = peerId});
        }
    }
}