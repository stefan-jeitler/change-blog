using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ChangeBlog.Api.Authentication;

public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
{
    private const string ApiKeyHeaderName = "X-Api-Key";
    private readonly FindUserId _findUserId;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<ApiKeyAuthenticationOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        FindUserId findUserId)
        : base(options, logger, encoder, clock)
    {
        _findUserId = findUserId;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyHeaderName, out var apiKeyHeaderValues))
            return AuthenticateResult.NoResult();

        var apiKeyInHeader = apiKeyHeaderValues.FirstOrDefault();

        if (apiKeyInHeader is null)
            return AuthenticateResult.NoResult();

        var userId = await _findUserId.FindByApiKeyAsync(apiKeyInHeader);
        if (!userId.HasValue || userId.Value == Guid.Empty)
            return AuthenticateResult.NoResult();

        return IssueTicket(userId.Value);
    }

    private static AuthenticateResult IssueTicket(Guid userId)
    {
        var identity = new ClaimsIdentity(new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId.ToString())
        }, ApiKeyAuthenticationOptions.AuthenticationType);

        var identities = new List<ClaimsIdentity> { identity };
        var principal = new ClaimsPrincipal(identities);
        var ticket = new AuthenticationTicket(principal, ApiKeyAuthenticationOptions.Scheme);

        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = MediaTypeNames.Application.Json;
        var responseBody = DefaultResponse.Create("You are not authenticated. Please enter a valid api key.");

        await Response.WriteAsync(JsonSerializer.Serialize(responseBody, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}