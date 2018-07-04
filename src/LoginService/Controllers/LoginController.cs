using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using JwtAuth.LoginService.Modules.Models;
using JwtAuth.LoginService.Modules.Repositories;
using JwtAuth.LoginService.Modules.Services;

namespace LoginService.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly TokenService _tokenService;

        public LoginController(UserRepository userRepository, TokenService tokenService)
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

            return Ok(_tokenService.GenerateToken(user.Username));
        }

        [HttpPost("validate")]
        public IActionResult Validate([FromBody]ValidateTokenRequest requestBody)
        {
            var user = _userRepository.GetUser(requestBody.Username);
            if (user == null)
            {
                return NotFound("The user was not found.");
            }

            var tokenUsername = _tokenService.ValidateToken(requestBody.Token);
            if (!$"{tokenUsername}".Equals(requestBody.Username, StringComparison.InvariantCultureIgnoreCase))
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
