using System;
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
        private readonly JwtService _service = new JwtService(NullLogger<JwtService>.Instance);

        [Fact]
        public void CreateToken()
        {
            string token = _service.CreateToken("au", "iss",
                TimeSpan.Parse("00:30:00"),
                new[] { new Claim(ClaimTypes.Email, "t_test@mail.com") },
                new AsymmetricSigning(GetJwtOptions())
            );
            Assert.NotEmpty(token);
        }

        [Fact]
        public void ValidateBaseToken()
        {
            var signing = new AsymmetricSigning(GetJwtOptions());
            string token = _service.CreateToken("au", "iss",
                TimeSpan.Parse("00:30:00"),
                new[] { new Claim(nameof(ClaimTypes.Email), "t_test@mail.com") },
                signing
            );

            JwtTokenResult result = _service.ValidateToken("au", "iss", token, signing.GetSecurityKey());
            Assert.True(result.Valid);
        }

        [Fact]
        public void SymmetricCreateToken()
        {
            string token = _service.CreateToken("au", "iss",
                TimeSpan.Parse("00:30:00"),
                new[] { new Claim(ClaimTypes.Email, "t_test@mail.com") },
                new SymmetricSecurity(GetJwtOptions())
            );
            Assert.NotEmpty(token);
        }

        [Fact]
        public void SymmetricValidateToken()
        {
            var signing = new SymmetricSecurity(GetJwtOptions());
            string token = _service.CreateToken("au", "iss",
                TimeSpan.Parse("00:30:00"),
                new[] { new Claim(nameof(ClaimTypes.Email), "t_test@mail.com") },
                signing
            );

            JwtTokenResult result = _service.ValidateToken("au", "iss", token, signing.GetSecurityKey());
            Assert.True(result.Valid);
        }

        private static IOptions<SigningSetting> GetJwtOptions()
        {
            var options = new Mock<IOptions<SigningSetting>>();
            options
                .Setup(mock => mock.Value)
                .Returns(new SigningSetting
                    {
                        PathPrivateKey = "./private.pem",
                        PathPublicKey = "./public.pem",
                        SecretKey = "secretsecretsecret"
                    }
                );
            return options.Object;
        }


    }
}
