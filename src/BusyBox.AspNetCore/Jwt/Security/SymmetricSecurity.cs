using System.Text;
using BusyBox.AspNetCore.Jwt.Exceptions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BusyBox.AspNetCore.Jwt.Security
{
    public class SymmetricSecurity : ISigning
    {
        private readonly string _secretKey;

        public SymmetricSecurity(IOptions<SigningSetting> options)
        {
            SigningSetting setting = options.Value;
            if (string.IsNullOrEmpty(setting.SecretKey))
                throw new SigningException("SecretKey cannot be empty");

            _secretKey = setting.SecretKey;
        }

        public SecurityKey GetSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        public SigningCredentials CreateSigning()
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            return new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        }
    }
}
