using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusyBox.AspNetCore.Jwt.Security;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Jwt.Services
{
    public class JwtService : IJwtService
    {
        private readonly ILogger<JwtService> _logger;

        public JwtService(ILogger<JwtService> logger)
        {
            _logger = logger;
        }

        public string CreateToken(
            string audience,
            string issuer,
            TimeSpan expires,
            IEnumerable<Claim> claims,
            ISigning signing)
        {
            if (string.IsNullOrEmpty(audience))
                throw new ArgumentNullException(nameof(audience));

            if (string.IsNullOrEmpty(issuer))
                throw new ArgumentNullException(nameof(issuer));

            if (expires == TimeSpan.Zero)
                throw new ArgumentNullException(nameof(expires));

            SigningCredentials signingCredentials = signing.CreateSigning();
            DateTime now = DateTime.UtcNow;
            JwtSecurityToken jwt = new (
                audience: audience,
                issuer: issuer,
                claims: claims,
                expires: now.Add(expires),
                signingCredentials: signingCredentials
            );

            try
            {
                JwtSecurityTokenHandler tokenHandler = new ();
                return tokenHandler.WriteToken(jwt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public JwtTokenResult ValidateToken(
            string audience,
            string issuer,
            string token,
            SecurityKey issuerSigningKey)
        {
            TokenValidationParameters validationParameters = new ()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = issuer,
                ValidAudience = audience
            };

            validationParameters.ValidateIssuerSigningKey = true;
            validationParameters.IssuerSigningKey = issuerSigningKey;
            validationParameters.CryptoProviderFactory =
                new CryptoProviderFactory { CacheSignatureProviders = false };

            try
            {
                JwtSecurityTokenHandler handler = new ();
                ClaimsPrincipal principal =
                    handler.ValidateToken(token, validationParameters, out SecurityToken _);
                return new JwtTokenResult(true, principal);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message);
                throw ex;
                //return JwtTokenResult.Empty;
            }
        }
    }
}
