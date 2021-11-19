using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared;
using ChangeBlog.Application.UseCases.Commands.AddExternalIdentity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Logging;

namespace ChangeBlog.Management.Api.Authentication;

public class AppAuthenticationHandler
{
    private readonly IAddExternalIdentity _addExternalIdentity;
    private readonly FindUserId _findUserId;
    private readonly ILogger<AppAuthenticationHandler> _logger;

    public AppAuthenticationHandler(FindUserId findUserId, IAddExternalIdentity addExternalIdentity,
        ILogger<AppAuthenticationHandler> logger)
    {
        _findUserId = findUserId;
        _addExternalIdentity = addExternalIdentity;
        _logger = logger;
    }

    public async Task HandleAsync(TokenValidatedContext context)
    {
        if (context.Principal is null)
        {
            context.Fail(string.Empty);
            return;
        }

        var externalUserId = context.Principal!.FindFirst(x => x.Type == ClaimTypes.NameIdentifier);
        if (externalUserId is null)
        {
            context.Fail(string.Empty);
            return;
        }

        await AddAppUserIdentity(context, externalUserId.Value);
    }

    private async Task AddAppUserIdentity(TokenValidatedContext context, string externalUserId)
    {
        var userId = await _findUserId.FindByExternalUserIdAsync(externalUserId);
        if (userId.HasValue)
        {
            AddAppIdentity(context, userId.Value);
            return;
        }

        var result = await _addExternalIdentity.ExecuteAsync(externalUserId);
        if (result.IsFailure)
        {
            _logger.LogCritical(result.Error);
            context.Fail(string.Empty);
            return;
        }

        AddAppIdentity(context, result.Value);
    }

    private static void AddAppIdentity(TokenValidatedContext context, Guid userId)
    {
        var appClaims = new List<Claim>(1)
        {
            new(Constants.AppClaims.UserId, userId.ToString())
        };
        var appIdentity = new ClaimsIdentity(appClaims);

        context.Principal!.AddIdentity(appIdentity);
    }
}