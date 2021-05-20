using System.Text;
using BusyBox.AspNetCore.Jwt.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Jwt.Security
{
    public class SymmetricSecurity : ISigning
    {
        private readonly IOptionsMonitor<JwtSecurityOptions> _options;

        public SymmetricSecurity(IOptionsMonitor<JwtSecurityOptions> options)
        {
            _options = options;
        }

        public SecurityKey GetSecurityKey()
        {
            JwtSecurityOptions setting = _options.CurrentValue;
            if (string.IsNullOrEmpty(setting.SecretKey))
                throw new SigningException("SecretKey cannot be empty");
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey));
        }

        public SigningCredentials CreateSigning()
        {
            JwtSecurityOptions setting = _options.CurrentValue;
            if (string.IsNullOrEmpty(setting.SecretKey))
                throw new SigningException("SecretKey cannot be empty");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(setting.SecretKey));

            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
