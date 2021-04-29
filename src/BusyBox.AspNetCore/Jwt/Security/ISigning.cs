using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Jwt.Security
{
    public interface ISigning
    {
        SecurityKey GetSecurityKey();

        SigningCredentials CreateSigning();
    }
}
