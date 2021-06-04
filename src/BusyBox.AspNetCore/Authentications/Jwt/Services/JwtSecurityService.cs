using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BusyBox.AspNetCore.Authentications.Jwt.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Authentications.Jwt.Services
{
    public class JwtSecurityService : IJwtSecurityService
    {
        private readonly ILogger<JwtSecurityService> _logger;
        private readonly IOptionsMonitor<JwtSecurityOptions> _optionsMonitor;
        private readonly ISigning _signing;

        /// <summary>
        /// Initializer for a new object
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="optionsMonitor"></param>
        /// <param name="signing"></param>
        public JwtSecurityService(ILogger<JwtSecurityService> logger,
            IOptionsMonitor<JwtSecurityOptions> optionsMonitor,
            ISigning signing)
        {
            _logger = logger;
            _optionsMonitor = optionsMonitor;
            _signing = signing;
        }

        /// <summary>
        /// Creates a signed JWT or JWE token
        /// </summary>
        /// <param name="claims">Claims that will be added to the token</param>
        /// <param name="signing">The <see cref="ISigning"/> that will be used to sign</param>
        /// <returns>Return JWT or JWE token</returns>
        /// <exception cref="ArgumentNullException">The (audience, issuer, expires) argument is null </exception>
        public string CreateToken(IEnumerable<Claim> claims)
        {
            SigningCredentials signingCredentials = _signing.CreateSigning();
            JwtSecurityOptions options = _optionsMonitor.CurrentValue;
            if (options == null)
                throw new ArgumentException("Missing config section for Jwt");

            DateTime now = DateTime.UtcNow;
            JwtSecurityToken jwt = new (
                audience: options.Audience,
                issuer: options.Issuer,
                claims: claims,
                expires: now.Add(TimeSpan.Parse(options.Expires!)),
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

        /// <summary>
        /// Reads and validates a 'JSON Web Token' (JWT) encoded as a JWS or JWE
        /// </summary>
        /// <param name="token"></param>
        /// <param name="issuerSigningKey">The <see cref="ISigning"/> that will be used to sign</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> from the JWT</returns>
        /// <exception cref="ArgumentNullException ">is not null audience</exception>
        /// <exception cref="ArgumentNullException ">is not null issuer</exception>
        /// <exception cref="ArgumentNullException ">is not null token</exception>
        public JwtTokenResult ValidateToken(
            string token,
            SecurityKey issuerSigningKey)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            JwtSecurityOptions options = _optionsMonitor.CurrentValue;
            if (options == null)
                throw new ArgumentException("Missing config section for Jwt");

            TokenValidationParameters validationParameters = new ()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = options.Issuer,
                ValidAudience = options.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = issuerSigningKey,
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };


            try
            {
                JwtSecurityTokenHandler handler = new ();
                ClaimsPrincipal principal =
                    handler.ValidateToken(token, validationParameters, out SecurityToken _);
                return new JwtTokenResult(true, principal);
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning(ex, ex.Message);
                return JwtTokenResult.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, ex.Message);
                throw;
                //return JwtTokenResult.Empty;
            }
        }
    }
}
