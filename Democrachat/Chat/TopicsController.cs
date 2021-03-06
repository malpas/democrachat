using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Chat
{
    [Route("/api/topics")]
    [ApiController]
    public class TopicsController : ControllerBase
    {
        private ITopicNameService _topicNameService;

        public TopicsController(ITopicNameService topicNameService)
        {
            _topicNameService = topicNameService;
        }

        [HttpGet]
        public IEnumerable<string> GetTopics()
        {
            return _topicNameService.GetTopics();
        }
    }
}