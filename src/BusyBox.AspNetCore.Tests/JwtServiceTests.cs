using System.Security.Claims;
using BusyBox.AspNetCore.Jwt;
using BusyBox.AspNetCore.Jwt.Security;
using BusyBox.AspNetCore.Jwt.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace BusyBox.AspNetCore.Tests
{
    public class JwtServiceTests
    {
        [Fact]
        public void CreateToken()
        {
            IJwtSecurityService service = CreateInstance();
            string token = service.CreateToken(
                new[] { new Claim(ClaimTypes.Email, "t_test@mail.com") },
                new AsymmetricSigning(GetJwtSecurityOptions())
            );
            Assert.NotEmpty(token);
        }

        [Fact]
        public void ValidateBaseToken()
        {
            var signing = new AsymmetricSigning(GetJwtSecurityOptions());
            IJwtSecurityService service = CreateInstance();

            string token = service.CreateToken(
                new[] { new Claim(nameof(ClaimTypes.Email), "t_test@mail.com") },
                signing
            );

            JwtTokenResult result = service.ValidateToken(token, signing.GetSecurityKey());
            Assert.True(result.Valid);
        }

        [Fact]
        public void SymmetricCreateToken()
        {
            IJwtSecurityService service = CreateInstance();
            string token = service.CreateToken(
                new[] { new Claim(ClaimTypes.Email, "t_test@mail.com") },
                new SymmetricSecurity(GetJwtSecurityOptions())
            );
            Assert.NotEmpty(token);
        }

        [Fact]
        public void SymmetricValidateToken()
        {
            IJwtSecurityService service = CreateInstance();
            var signing = new SymmetricSecurity(GetJwtSecurityOptions());
            string token = service.CreateToken(
                new[] { new Claim(nameof(ClaimTypes.Email), "t_test@mail.com") },
                signing
            );

            JwtTokenResult result = service.ValidateToken(token, signing.GetSecurityKey());
            Assert.True(result.Valid);
        }

        private static IJwtSecurityService CreateInstance() =>
            new JwtSecurityService(
                NullLogger<JwtSecurityService>.Instance,
                GetJwtSecurityOptions()
            );

        private static IOptionsMonitor<JwtSecurityOptions> GetJwtSecurityOptions()
        {
            var options = new Mock<IOptionsMonitor<JwtSecurityOptions>>();
            options
                .SetupGet(mock => mock.CurrentValue)
                .Returns(new JwtSecurityOptions
                    {
                        Audience = "au",
                        Issuer = "iss",
                        Expires = "00:30:00",
                        PathPrivateKey = "./private.pem",
                        PathPublicKey = "./public.pem",
                        SecretKey = "secretsecretsecret",
                    }
                );

            return options.Object;
        }
    }
}
