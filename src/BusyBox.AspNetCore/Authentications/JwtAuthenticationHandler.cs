using System;
using System.Security;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using BusyBox.AspNetCore.Jwt;
using BusyBox.AspNetCore.Jwt.Security;
using BusyBox.AspNetCore.Jwt.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace BusyBox.AspNetCore.Authentications
{
    public class JwtAuthenticationHandler : AuthenticationHandler<JwtAuthenticationOptions>
    {
        public const string SchemaName = "Bearer";
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

            if (!authorizationHeader.StartsWith(SchemaName, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(AuthenticateResult.Fail(Unauthorized));

            string token = authorizationHeader.Substring(SchemaName.Length).Trim();

            try
            {
                JwtTokenResult result = _securityService.ValidateToken(token, _signing.GetSecurityKey());
                if (!result.Valid)
                    return Task.FromResult(AuthenticateResult.Fail(Unauthorized));

                if (result.Principal == null)
                    throw new SecurityException("Principal is null");

                var claims = ((ClaimsIdentity)result.Principal.Identity).Claims;

                ClaimsIdentity identity = new (claims, Scheme.Name);
                GenericPrincipal principal = new (identity, null);
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
