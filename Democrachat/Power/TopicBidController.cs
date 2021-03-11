using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Power
{
    [ApiController]
    [Authorize]
    [Route("/api/topic")]
    public class TopicBidController : ControllerBase
    {
        private TopicBidService _topicBidService;

        public TopicBidController(TopicBidService topicBidService)
        {
            _topicBidService = topicBidService;
        }
        
        [HttpPost]
        [Route("bid")]
        public IActionResult Bid([FromBody] TopicBidRequest request)
        {
            var id = int.Parse(User.FindFirstValue("Id"));
            var result = _topicBidService.Bid(id, request.Name, request.Silver);
            return result switch
            {
                TopicBidService.Result.NOT_ENOUGH_SILVER => BadRequest("Not enough silver"),
                TopicBidService.Result.NO_USER_DATA => BadRequest("Where is the data"),
                _ => Ok($"Bid {request.Silver} silver for a \"{request.Name}\" topic")
            };
        }
    }
}