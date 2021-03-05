using System.Security.Claims;
using System.Threading.Tasks;
using Democrachat.Auth.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Democrachat.Auth
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {
        private IAuthService _authService;
        private RegisterSpamCheckService _registerSpamCheckService;

        public AuthController(IAuthService authService, RegisterSpamCheckService registerSpamCheckService)
        {
            _authService = authService;
            _registerSpamCheckService = registerSpamCheckService;
        }
        
        /// <summary>
        /// Login a user via username and password
        /// </summary>
        /// <param name="login">Login request data</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            var userData = _authService.AttemptLogin(login.Username, login.Password);
            if (userData == null)
                return BadRequest("Invalid username or password");
            
            var identity = new ClaimsIdentity(new [] {new Claim("Id", userData.Id.ToString())},
                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return Ok("Logged in");
        }

        /// <summary>
        /// Register a user using proprietary magic "one-click register" guest system. Should be able to be
        /// "finalized" later to give it a username and password.
        /// </summary>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register()
        {
            if (User.Identity.IsAuthenticated)
            {
                return BadRequest("Already logged in");
            }
            if (!_registerSpamCheckService.CheckIp(HttpContext.Connection.RemoteIpAddress))
            {
                return BadRequest("You created a guest account recently");
            }
            var result = _authService.RegisterUser();
            var identity = new ClaimsIdentity(new [] {new Claim("Id", result.UserId.ToString())},
                CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(principal);
            return Ok();
        }

        /// <summary>
        /// Get user info.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet("info")]
        public IActionResult Info()
        {
            var id = HttpContext.User.FindFirstValue("Id");
            var userData = _authService.GetUserById(int.Parse(id));
            return Ok(userData);
        }

        /// <summary>
        /// Logout a signed in user.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Ok();
        }

        /// <summary>
        /// Convert a new "guest" account into a regular account with a username and password (the password
        /// hopefully allowing the account to be logged into).
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("finalize")]
        public IActionResult Finalize([FromBody] Register register)
        {
            var id = int.Parse(HttpContext.User.FindFirstValue("Id"));
            if (_authService.IsUsernameTaken(register.Username))
            {
                return BadRequest($"User \"{register.Username}\" is already taken");
            }
            _authService.FinalizeNewUser(id, register.Username, register.Password);
            return Ok("Welcome");
        }
    }
}