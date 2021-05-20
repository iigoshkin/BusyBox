using BusyBox.AspNetCore.Jwt.Security;
using Microsoft.Extensions.DependencyInjection;

namespace BusyBox.AspNetCore.Authentications
{
    public class JwtAuthenticationBuilder
    {
        private readonly IServiceCollection _services;

        public JwtAuthenticationBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public JwtAuthenticationBuilder AddAsymmetricSigning()
        {
            _services.AddSingleton<ISigning, AsymmetricSigning>();
            return this;
        }

        public JwtAuthenticationBuilder AddSymmetricSecurity()
        {
            _services.AddSingleton<ISigning, SymmetricSecurity>();
            return this;
        }
    }
}
