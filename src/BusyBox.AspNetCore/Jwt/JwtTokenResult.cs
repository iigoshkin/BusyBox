using System.Security.Claims;

namespace BusyBox.AspNetCore.Jwt
{
    public class JwtTokenResult
    {
        public bool Valid { get; }

        public ClaimsPrincipal? Principal { get; }

        public JwtTokenResult(bool valid, ClaimsPrincipal? principal)
        {
            Valid = valid;
            Principal = principal;
        }

        public static JwtTokenResult Empty => new (false, null);
    }
}
