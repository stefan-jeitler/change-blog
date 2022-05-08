using System.Collections.Generic;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ChangeBlog.Api.Authentication;

public static class JwtAuthUtils
{
    public static Task Challenge(JwtBearerChallengeContext context)
    {
        context.Response.OnStarting(async () =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger>();
            
            var authException = context.AuthenticateFailure;
            if(authException is not null)
                logger.LogCritical(authException, "Error while authenticating user.");

            context.Response.ContentType = MediaTypeNames.Application.Json;
            var responseBody = ErrorResponse.Create(ChangeBlogStrings.NotAuthenticated);

            await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        });

        return Task.CompletedTask;
    }
    
    public static async Task IssueAppTicket(TokenValidatedContext context)
    {
        var findUserId = context.HttpContext.RequestServices.GetRequiredService<FindUserId>();
        var externalUserId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            context.Fail(ChangeBlogStrings.MissingSubClaim);
            return;
        }

        var appUserId = await findUserId.FindByExternalUserIdAsync(externalUserId);

        if (!appUserId.HasValue)
        {
            context.Fail(ChangeBlogStrings.UserNotExistingInApp);
            return;
        }

        var appIdentity = new ClaimsIdentity(new List<Claim>
        {
            new(ApiConstants.AppClaims.UserId, appUserId!.Value.ToString())
        }, "ChangeBlogApp");

        context.Principal!.AddIdentity(appIdentity);
    }
}