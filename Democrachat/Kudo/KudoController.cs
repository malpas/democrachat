using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Kudo
{
    [ApiController]
    [Route("/api/kudo")]
    public class KudoController : ControllerBase
    {
        private IKudoService _kudoService;

        public KudoController(IKudoService kudoService)
        {
            _kudoService = kudoService;
        }
        
        [HttpPost]
        public IActionResult GiveKudo([FromBody] KudoRequest request)
        {
            var userId = int.Parse(User.FindFirstValue("Id"));
            try
            {
                _kudoService.GiveKudo(userId, request.Username, HttpContext.Connection.RemoteIpAddress);
            }
            catch (InvalidOperationException e)
            {
                return BadRequest(new {Error = e.Message});
            }

            return Ok();
        }
    }
}