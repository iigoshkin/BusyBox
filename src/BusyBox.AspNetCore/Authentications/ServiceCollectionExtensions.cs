using System;
using BusyBox.AspNetCore.Jwt;
using BusyBox.AspNetCore.Jwt.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusyBox.AspNetCore.Authentications
{
    public static class ServiceCollectionExtensions
    {
        public static JwtAuthenticationBuilder AddJwt(
            this IServiceCollection services,
            IConfiguration configuration,
            string section = "JWT",
            string schema = "Bearer"
        )
        {
            if (string.IsNullOrEmpty(section))
                throw new ArgumentNullException(nameof(section));

            if (string.IsNullOrEmpty(schema))
                throw new ArgumentNullException(nameof(schema));

            services.Configure<JwtSecurityOptions>(configuration.GetSection(section));
            services.AddSingleton<IJwtSecurityService, JwtSecurityService>();
            services.AddAuthentication()
                .AddScheme<JwtAuthenticationOptions, JwtAuthenticationHandler>(
                    schema,
                    null);

            return new JwtAuthenticationBuilder(services);
        }
    }
}
