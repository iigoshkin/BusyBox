using System;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BusyBox.AspNetCore.Authentications.Jwt.Security;
using BusyBox.AspNetCore.Authentications.Jwt.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace BusyBox.AspNetCore.Authentications.Jwt.Handler
{
    internal class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
    {
        private const string Unauthorized = "Unauthorized";

        private readonly IJwtSecurityService _securityService;
        private readonly ISigning _signing;

        public JwtAuthenticationHandler(
            IJwtSecurityService securityService,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IOptionsMonitor<JwtAuthenticationOptions> options,
            ISystemClock clock, ISigning signing)
            : base(options, logger, encoder, clock)
        {
            _securityService = securityService;
            _signing = signing;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey(HeaderNames.Authorization))
                return Task.FromResult(AuthenticateResult.Fail(Unauthorized));

            string authorizationHeader = Request.Headers[HeaderNames.Authorization];
            if (string.IsNullOrEmpty(authorizationHeader))
                return Task.FromResult(AuthenticateResult.NoResult());

            if (!authorizationHeader.StartsWith(Scheme.Name, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.Fail(Unauthorized));

            string token = authorizationHeader.Substring(Scheme.Name.Length).Trim();

            try
            {
                JwtTokenResult result = _securityService.ValidateToken(token, _signing.GetSecurityKey());
                if (!result.Valid)
                    return Task.FromResult(AuthenticateResult.Fail(Unauthorized));

                if (result.Principal == null)
                    throw new SecurityException("Principal is null");

                var currentIdentity = result.Principal.Identity as ClaimsIdentity;
                if (currentIdentity == null)
                    throw new Exception("Is not type ClaimsIdentity");

                var claims = currentIdentity.Claims;

                ClaimsIdentity identity = new (claims, Scheme.Name);
                GenericPrincipal principal = new (currentIdentity, null);
                AuthenticationTicket ticket = new (principal, Scheme.Name);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (Exception ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex.Message));
            }
        }
    }
}
