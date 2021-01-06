using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using JwtAuth.LoginService.Modules.Models;

namespace JwtAuth.LoginService.Modules.Services
{
    public class TokenService
    {
        private static string Secret = "g3TkuNgOkSjmGW7yD1w/rI+Saqd1eAmxPid8VQcuJKa1DBz4w3oQyhas1uSm45XMVyW4RBOscbdGzirpfIMukQ==";

        public string GenerateToken(User user)
        {
            byte[] key = Convert.FromBase64String(Secret);
            var securityKey = new SymmetricSecurityKey(key);

            var claims = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                    // fixed data is preferred so you dont have to worry about out of date claims
            });

            if (user.UserRole.Any())
            {
                foreach (var role in user.UserRole)
                {
                    claims.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                }
            }
            
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(securityKey,
                SecurityAlgorithms.HmacSha256Signature)
            };
        
            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);
            token.Payload["UserData"] = "49"; // TODO: figure out standard way to add payload. "UserData" could conflict with actual ClaimTypes.UserData
            
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

        public enum InvalidTokenReason
        {
            Expired,
            Fake,
            Unauthorized
        }

        public User ValidateToken(string token, out InvalidTokenReason? invalidTokenReason)
        {
            var principal = GetPrincipal(token);
            if (principal == null)
            {
                invalidTokenReason = InvalidTokenReason.Fake;
                return null;
            }

            try
            {
                var identity = (ClaimsIdentity)principal.Identity;

                var isAuthenticated = identity.IsAuthenticated;
                if (!isAuthenticated)
                {
                    invalidTokenReason = InvalidTokenReason.Unauthorized;
                }

                var (isExpired, dateOfExpiration) = IsTokenExpired(identity);
                if (isExpired)
                {
                    invalidTokenReason = InvalidTokenReason.Expired;
                }
                
                var usernameClaim = identity.FindFirst(ClaimTypes.Name);
                var user = new User()
                {
                    Username = usernameClaim.Value
                };

                var roleClaims = identity.FindAll(ClaimTypes.Role);
                var roles = new List<UserRole>();
                foreach (var roleClaim in roleClaims)
                {
                    var isRealRole = Enum.TryParse(typeof(UserRole), roleClaim.Value, out var role);
                    if (isRealRole)
                    {
                        roles.Add((UserRole)role);
                    }
                }
                user.UserRole = roles.ToArray();

                invalidTokenReason = null;
                return user;
            }
            catch (Exception)
            {
                invalidTokenReason = null;
                return null;
            }

            (bool isExpired, DateTime? dateOfExpiration) IsTokenExpired(ClaimsIdentity unverifiedIdentity)
            {
                var expClaim = unverifiedIdentity.FindFirst("exp");
                
                var unixDateMin = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
                var validTo = string.IsNullOrWhiteSpace(expClaim.Value)
                    ? unixDateMin
                    : unixDateMin.AddMilliseconds(long.Parse(expClaim.Value));
                
                Console.WriteLine($"Token valid to: {validTo}");
                
                if (DateTime.Compare(validTo, DateTime.UtcNow) <= 0)
                {
                    return (true, validTo);
                }

                return (false, null);
            }
        }
    } 
}
