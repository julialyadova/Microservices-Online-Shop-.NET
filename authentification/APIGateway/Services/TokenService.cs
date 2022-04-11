using System;
using APIGateway.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using APIGateway.Config;
using APIGateway.Data;
using System.Linq;
using System.Text.Json;

namespace APIGateway.Services
{
    public class TokenService
    {
        private JWTConfig _jwtConfig;

        public TokenService(IOptions<JWTConfig> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var expirationDate = DateTime.UtcNow.AddMinutes(3);

            var userData = new UserModel()
            {
                Id = user.Id,
                Username = user.Username,
                Roles = user.Roles?.Select(r => r.Name).ToArray(),
            };

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.UserData, JsonSerializer.Serialize(userData))
            };

            var token = new JwtSecurityToken(audience: "usersAudience",
                                              issuer: "usersIssuer",
                                              claims: claims,
                                              expires: expirationDate,
                                              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}