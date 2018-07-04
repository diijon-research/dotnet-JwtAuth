using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace JwtAuth.LoginService.Modules.Services
{
    public class TokenService
    {
        private static string Secret = "g3TkuNgOkSjmGW7yD1w/rI+Saqd1eAmxPid8VQcuJKa1DBz4w3oQyhas1uSm45XMVyW4RBOscbdGzirpfIMukQ==";

        public string GenerateToken(string username)
        {
            byte[] key = Convert.FromBase64String(Secret);
            var securityKey = new SymmetricSecurityKey(key);
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] 
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };
        
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);
            token.Payload["extraData"] = "foo";
            return handler.WriteToken(token);
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null)
                {
                    return null;
                }

                byte[] key = Convert.FromBase64String(Secret);
                var parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out var securityToken);
                return principal;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string ValidateToken(string token)
        {
            var principal = GetPrincipal(token);
            if (principal == null)
            {
                return null;
            }

            try
            {
                var identity = (ClaimsIdentity)principal.Identity;
                var usernameClaim = identity.FindFirst(ClaimTypes.Name);
                var username = usernameClaim.Value;
                return username;
            }
            catch (Exception)
            {
                return null;
            }
        }
    } 
}
