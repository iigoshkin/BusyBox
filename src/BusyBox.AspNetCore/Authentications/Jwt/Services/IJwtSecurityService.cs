using System;
using System.Collections.Generic;
using System.Security.Claims;
using BusyBox.AspNetCore.Authentications.Jwt.Security;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Authentications.Jwt.Services
{
    public interface IJwtSecurityService
    {
        /// <summary>
        /// Creates a signed JWT or JWE token
        /// </summary>
        /// <param name="audience">Identifies the intended recipient of the token.</param>
        /// <param name="issuer">Identifies the security token service (STS) that constructs and returns the token</param>
        /// <param name="expires">Identifies the expiration time on or after which the JWT MUST NOT be accepted for processing</param>
        /// <param name="claims">Claims that will be added to the token</param>
        /// <param name="signing">The <see cref="ISigning"/> that will be used to sign</param>
        /// <returns>Return JWT or JWE token</returns>
        /// <exception cref="ArgumentNullException">The (audience, issuer, expires) argument is null </exception>
        string CreateToken(
            IEnumerable<Claim> claims,
            ISigning asymmetricSigning);

        /// <summary>
        /// Reads and validates a 'JSON Web Token' (JWT) encoded as a JWS or JWE
        /// </summary>
        /// <param name="audience">Identifies the intended recipient of the token</param>
        /// <param name="issuer">Identifies the security token service (STS) that constructs and returns the token</param>
        /// <param name="token"></param>
        /// <param name="issuerSigningKey">The <see cref="ISigning"/> that will be used to sign</param>
        /// <returns>A <see cref="ClaimsPrincipal"/> from the JWT</returns>
        /// <exception cref="ArgumentNullException ">is not null audience</exception>
        /// <exception cref="ArgumentNullException ">is not null issuer</exception>
        /// <exception cref="ArgumentNullException ">is not null token</exception>
        JwtTokenResult ValidateToken(
            string token,
            SecurityKey issuerSigningKey);
    }
}
