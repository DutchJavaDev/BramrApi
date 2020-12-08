using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BramrApi.Utils
{
    public static class IdentityConfig
    {
        public static string JWT_KEY = string.Empty;
        public static string JWT_ISSUER = string.Empty;
        private const double JWT_EXPIRE_DAYS = 7;

        public static readonly List<string> ApiRoles = new List<string> {};

        public static void Init(IConfiguration configuration)
        {
            JWT_KEY = configuration["JwtKey"];
#if DEBUG
            JWT_ISSUER = configuration["JwtIssuerDev"];
#else
            JWT_ISSUER = configuration["JwtIssuer"];
#endif
        }

        /// <summary>
        /// Generates a JWT token that can be used for authentication
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userRoles"></param>
        /// <returns></returns>
        public static string GenerateJWT(IdentityUser user, IList<string> userRoles = null)
        {
            var userClaims = new List<Claim> {
                new Claim(JwtRegisteredClaimNames.Jti, GenerateJTI()),
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
            };

            if (userRoles != null)
            {
                foreach (var role in userRoles)
                {
                    userClaims.Add(new Claim(ClaimTypes.Role, role));
                }
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT_KEY));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(JWT_EXPIRE_DAYS);

            var token = new JwtSecurityToken(
                JWT_ISSUER,
                JWT_ISSUER,
                userClaims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateJTI()
        {
            return new Guid().ToString();
        }
    }
}
