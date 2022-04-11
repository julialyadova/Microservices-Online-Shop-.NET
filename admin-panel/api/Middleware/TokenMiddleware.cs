using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace Api.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var tokenHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            var token = tokenHeader.Replace("Bearer ", "");
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var jti = jwtSecurityToken.Claims.First(claim => claim.Type == ClaimTypes.UserData).Value;
            var userData = JsonSerializer.Deserialize<UserModel>(jti);

            if (userData.Roles == null || !userData.Roles.Contains("Admin"))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Only users with role Admin are allowed");
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }

    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public ICollection<string> Roles { get; set; }
    }
}
