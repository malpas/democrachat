using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Log
{
    [ApiController]
    [Authorize]
    [Route("/api/log")]
    public class LogController : ControllerBase
    {
        private ILogger _logger;

        public LogController(ILogger logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult GetLog()
        {
            return Ok(_logger.ReadLog());
        }
    }
}