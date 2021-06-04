using System.Security.Claims;
using BusyBox.AspNetCore.Authentications.Jwt;
using BusyBox.AspNetCore.Authentications.Jwt.Security;
using BusyBox.AspNetCore.Authentications.Jwt.Services;
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
            var signing = new AsymmetricSigning(GetJwtSecurityOptions());
            IJwtSecurityService service = CreateInstance(signing);
            string token = service.CreateToken(
                new[] { new Claim(ClaimTypes.Email, "t_test@mail.com") }
            );
            Assert.NotEmpty(token);
        }

        [Fact]
        public void ValidateBaseToken()
        {
            var signing = new AsymmetricSigning(GetJwtSecurityOptions());
            IJwtSecurityService service = CreateInstance(signing);

            string token = service.CreateToken(
                new[] { new Claim(nameof(ClaimTypes.Email), "t_test@mail.com") }
            );

            JwtTokenResult result = service.ValidateToken(token, signing.GetSecurityKey());
            Assert.True(result.Valid);
        }

        [Fact]
        public void SymmetricCreateToken()
        {
            var signing = new SymmetricSecurity(GetJwtSecurityOptions());
            IJwtSecurityService service = CreateInstance(signing);
            string token = service.CreateToken(
                new[] { new Claim(ClaimTypes.Email, "t_test@mail.com") }
            );
            Assert.NotEmpty(token);
        }

        [Fact]
        public void SymmetricValidateToken()
        {
            var signing = new SymmetricSecurity(GetJwtSecurityOptions());
            IJwtSecurityService service = CreateInstance(signing);

            string token = service.CreateToken(
                new[] { new Claim(nameof(ClaimTypes.Email), "t_test@mail.com") }
            );

            JwtTokenResult result = service.ValidateToken(token, signing.GetSecurityKey());
            Assert.True(result.Valid);
        }

        private static IJwtSecurityService CreateInstance(ISigning signing) =>
            new JwtSecurityService(
                NullLogger<JwtSecurityService>.Instance,
                GetJwtSecurityOptions(),
                signing
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
