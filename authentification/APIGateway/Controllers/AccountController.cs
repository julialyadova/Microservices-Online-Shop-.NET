using APIGateway.Models;
using APIGateway.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace APIGateway.Account.Controllers
{
    [Route("/")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private AccountService _accountService;
        private TokenService _tokenService;

        public AccountController(AccountService accountService, TokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [HttpPost("/register")]
        public IActionResult Register([FromBody] RegisterModel registerModel)
        {
            var result = _accountService.Register(registerModel);
            if (result.Successful)
            {
                var token = _tokenService.GenerateToken(result.Value);
                Response.Headers.Append("Authorization", $"Bearer {token}");
                return Ok();
            }
            else
                return BadRequest(result.Message);
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var result = _accountService.Login(loginModel);
            if (result.Successful)
            {
                var token = _tokenService.GenerateToken(result.Value);
                Response.Headers.Append("Authorization", $"Bearer {token}");
                return Ok();
            }
            else
                return BadRequest(result.Message);
        }
    }
}
