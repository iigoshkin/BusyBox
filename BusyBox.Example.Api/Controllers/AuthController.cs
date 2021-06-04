using System.Security.Claims;
using BusyBox.AspNetCore.Authentications.Jwt.Security;
using BusyBox.AspNetCore.Authentications.Jwt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusyBox.Example.Api.Controllers
{
    [Route("/api/v1/auth")]
    public class AuthController : Controller
    {
        private readonly IJwtSecurityService _service;
        private readonly ISigning _signing;

        public AuthController(IJwtSecurityService service, ISigning signing)
        {
            _service = service;
            _signing = signing;
        }

        [HttpGet("token")]
        public string GetToken() => _service.CreateToken(new[] { new Claim(ClaimTypes.Email, "u_primer@primer.net") }, _signing);

        [HttpGet("message")]
        [Authorize]
        public string GetMessage() => User.FindFirst(ClaimTypes.Email)?.Value ?? "empty";

        [HttpGet("message/alfa")]
        [Authorize(AuthenticationSchemes = "Alfa")]
        public string GetMessageAlfa() => User.FindFirst(ClaimTypes.Email)?.Value ?? "empty";
    }
}
