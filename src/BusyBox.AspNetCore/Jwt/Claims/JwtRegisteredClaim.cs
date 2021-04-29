using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BusyBox.AspNetCore.Jwt.Claims
{
    public class JwtRegisteredClaim
    {
        public Claim GetIat(long unixTotalSecond) =>
            new (JwtRegisteredClaimNames.Iat, unixTotalSecond.ToString(), ClaimValueTypes.Integer64);

        public Claim GetJti() => new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString());
    }
}
