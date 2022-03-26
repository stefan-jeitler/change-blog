using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.Authentication;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Constants = ChangeBlog.Api.Shared.Constants;

namespace ChangeBlog.Api.Authentication;

public static class ServiceCollectionExtensions
{
    private const string PolicyScheme = "SchemePicker";

    public static IServiceCollection AddAppAuthentication(this IServiceCollection services,
        MicrosoftIdentityAuthenticationSettings settings)
    {
        services.AddScoped<FindUserId>();
        services.AddAuthentication(PolicyScheme)
            .AddPolicyScheme(PolicyScheme, "Authorization JWT or Api Key", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    return authHeader is not null && authHeader.StartsWith("Bearer ")
                        ? JwtBearerDefaults.AuthenticationScheme
                        : ApiKeyAuthenticationOptions.Scheme;
                };
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyAuthenticationOptions.Scheme, _ => { })
            .AddMicrosoftIdentityWebApi(o =>
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = OnTokenValidated,
                    OnChallenge = OnChallenge
                }, o =>
            {
                o.Instance = settings.Instance;
                o.TenantId = settings.TenantId;
                o.ClientId = settings.ClientId;
                o.ClientSecret = settings.ClientSecret;
            })
            .EnableTokenAcquisitionToCallDownstreamApi(_ => { })
            .AddInMemoryTokenCaches();

        return services;
    }

    private static async Task OnTokenValidated(TokenValidatedContext context)
    {
        var findUserId = context.HttpContext.RequestServices.GetRequiredService<FindUserId>();
        var externalUserId = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(externalUserId))
        {
            context.Fail("Missing 'sub' claim.");
            return;
        }

        var appUserId = await findUserId.FindByExternalUserIdAsync(externalUserId);

        if (!appUserId.HasValue)
        {
            context.Fail("User does not exist in app.");
            return;
        }

        var appIdentity = new ClaimsIdentity(new List<Claim>
        {
            new(Constants.AppClaims.UserId, appUserId!.Value.ToString())
        });

        context.Principal!.AddIdentity(appIdentity);
    }

    private static Task OnChallenge(JwtBearerChallengeContext context)
    {
        context.Response.OnStarting(async () =>
        {
            var message = context.AuthenticateFailure?.Message ?? "Please add a valid JWT Bearer Token.";

            context.Response.ContentType = MediaTypeNames.Application.Json;
            var responseBody = DefaultResponse.Create($"You are not authenticated. {message}");

            await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));
        });

        return Task.CompletedTask;
    }
}