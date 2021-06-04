using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Authentications.Jwt.Security
{
    public interface ISigning
    {
        SecurityKey GetSecurityKey();

        SigningCredentials CreateSigning();
    }
}
