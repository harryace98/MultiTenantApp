using Application.Abstractions.Authentication;
using Domain.Models;
using Infrastructure.Authorization.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Authentication
{
    internal sealed class TokenProvider(IConfiguration configuration) : ITokenProvider
    {

        public string GenerateJWTToken(User user, ImmutableHashSet<string> permissions)
        {
            string secretKey = configuration["Jwt:Secret"]!;
            int expireTime = int.Parse(configuration["Jwt:ExpirationInMinutes"]!); // Replace GetValue with int.Parse
            string issuerToken = configuration["Jwt:Issuer"]!;
            string audienceToken = configuration["Jwt:Audience"]!;

            if (string.IsNullOrEmpty(secretKey))
                throw new ArgumentNullException("The secret Key or the Expire Time of the Token Generator have not been configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),        
                new Claim(JwtRegisteredClaimNames.Email, user.Email),              
                new Claim(ClaimTypes.Role, GetRoleFromUser(user)),                 
                new Claim(CustomClaimTypes.TENANT_ID, user.TenantId),               
                new Claim(CustomClaimTypes.PERMISSIONS, JsonSerializer.Serialize(permissions))
            };

            var tokenDescritor = new JwtSecurityToken(
                issuer: issuerToken,
                audience: audienceToken,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireTime),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenDescritor);
        }

        public IUserContext ValidateToken(string token)
        {
            if (token == null)
                return null;

            string secretKey = configuration["Jwt:Secret"]!;
            int expireTime = int.Parse(configuration["Jwt:ExpirationInMinutes"]!); // Replace GetValue with int.Parse
            string issuerToken = configuration["Jwt:Issuer"]!;
            string audienceToken = configuration["Jwt:Audience"]!;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value);

            // return user id from JWT token if validation successful
            var roles = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value.Split(',');
            return new UserContext
            {
                Email = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Email).Value,
                Roles = jwtToken.Claims.First(x => x.Type == ClaimTypes.Role).Value.Split(','),
                Id = int.Parse(jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.NameId).Value),
            };
        }
        private static string GetRoleFromUser(User user)
        {
            return string.Join(",", user.UserRoles.Select(x => x.RoleId).ToList());
        }
    }
}
