using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
//using System.Web.Http;

using JwtAuth.LoginService.Modules.Models;
using JwtAuth.LoginService.Modules.Repositories;
using JwtAuth.LoginService.Modules.Services;

namespace LoginService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly TokenService _tokenService;

        public AuthController(UserRepository userRepository, TokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost]
        public IActionResult Login([FromBody]User user)
        {
            var u = _userRepository.GetUser(user.Username);
            if (u == null)
            {
                return NotFound("The user was not found");
            }

            var credentials = u.Password.Equals(user.Password);
            if (!credentials)
            {
                return StatusCode((int)HttpStatusCode.Forbidden, "The username/password combination was wrong."); 
            }

            return Ok(_tokenService.GenerateToken(u));
        }

        [HttpPost]
        public IActionResult Validate([FromBody]ValidateTokenRequest requestBody)
        {
            var user = _userRepository.GetUser(requestBody.Username);
            if (user == null)
            {
                return NotFound("The user was not found.");
            }

            var tokenUser = _tokenService.ValidateToken(requestBody.Token, out var invalidTokenReason);
            var userMatch =
                tokenUser.Username.Equals(requestBody.Username, StringComparison.InvariantCultureIgnoreCase);
            if (!userMatch || invalidTokenReason.HasValue)
            {
                return BadRequest(new
                {
                    Reason = invalidTokenReason.ToString(),
                    UserMatch = userMatch
                });
            }

            return Ok(tokenUser);
        }
    }
}
