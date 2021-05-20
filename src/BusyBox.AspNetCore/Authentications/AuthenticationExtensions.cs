using System;
using BusyBox.AspNetCore.Jwt;
using BusyBox.AspNetCore.Jwt.Security;
using BusyBox.AspNetCore.Jwt.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusyBox.AspNetCore.Authentications
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddJwt(
            this AuthenticationBuilder authenticationBuilder,
            IConfiguration configuration,
            string section = "JWT",
            string schema = "Bearer"
        )
        {
            if (string.IsNullOrEmpty(section))
                throw new ArgumentNullException(nameof(section));

            if (string.IsNullOrEmpty(schema))
                throw new ArgumentNullException(nameof(schema));

            IServiceCollection services = authenticationBuilder.Services;
            services.Configure<JwtSecurityOptions>(configuration.GetSection(section));
            services.AddSingleton<IJwtSecurityService, JwtSecurityService>();
            services.AddAuthentication()
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(
                    schema,
                    null);

            return authenticationBuilder;
        }

        public static AuthenticationBuilder AddAsymmetricSigning(this AuthenticationBuilder authenticationBuilder)
        {
            IServiceCollection services = authenticationBuilder.Services;
            services.AddSingleton<ISigning, AsymmetricSigning>();
            return authenticationBuilder;
        }

        public static AuthenticationBuilder AddSymmetricSecurity(this AuthenticationBuilder authenticationBuilder)
        {
            IServiceCollection services = authenticationBuilder.Services;
            services.AddSingleton<ISigning, SymmetricSecurity>();
            return authenticationBuilder;
        }
    }
}
