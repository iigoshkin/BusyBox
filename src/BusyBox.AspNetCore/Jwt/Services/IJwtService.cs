using System;
using System.Collections.Generic;
using System.Security.Claims;
using BusyBox.AspNetCore.Jwt.Security;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Jwt.Services
{
    public interface IJwtService
    {
        string CreateToken(
            string audience,
            string issuer,
            TimeSpan expires,
            IEnumerable<Claim> claims,
            ISigning asymmetricSigning);

        JwtTokenResult ValidateToken(
            string issuer,
            string audience,
            string token,
            SecurityKey issuerSigningKey);
    }
}
