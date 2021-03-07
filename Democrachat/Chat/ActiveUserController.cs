using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Chat
{
    [ApiController]
    [Route("/api/topicActivity")]
    [Authorize]
    public class ActiveUserController : ControllerBase
    {
        private ActiveUserService _activeUserService;

        public ActiveUserController(ActiveUserService activeUserService)
        {
            _activeUserService = activeUserService;
        }
        
        [HttpGet("{topic}")]
        public IActionResult GetUsersInTopic(string topic)
        {
            return Ok(_activeUserService.GetUsersInTopic(topic));
        }
    }
}