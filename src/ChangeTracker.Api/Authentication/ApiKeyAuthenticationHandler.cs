using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

// ReSharper disable InvertIf

namespace ChangeTracker.Api.Authentication
{
    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private const string ApiKeyHeaderName = "X-Api-Key";

        private readonly IAccountDao _accountDao;

        public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IAccountDao accountDao)
            : base(options, logger, encoder, clock)
        {
            _accountDao = accountDao;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
                return AuthenticateResult.NoResult();

            var apiKeyInHeader = apiKeyHeaderValues.FirstOrDefault();

            if (apiKeyInHeader is null)
                return AuthenticateResult.NoResult();

            var accountId = await _accountDao.FindAccountId(apiKeyInHeader);
            if (accountId.HasValue)
            {
                var identity = new ClaimsIdentity(new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, accountId.ToString())
                }, Options.AuthenticationType);

                var identities = new List<ClaimsIdentity> {identity};
                var principal = new ClaimsPrincipal(identities);
                var ticket = new AuthenticationTicket(principal, Options.Scheme);

                return AuthenticateResult.Success(ticket);
            }

            return AuthenticateResult.NoResult();
        }
    }
}