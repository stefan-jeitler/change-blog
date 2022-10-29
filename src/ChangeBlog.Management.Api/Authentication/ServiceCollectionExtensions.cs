using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.Authentication;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;

namespace ChangeBlog.Management.Api.Authentication;

public static class ServiceCollectionExtensions
{
    public static AuthenticationBuilder AddAppAuthentication(this AuthenticationBuilder authBuilder,
        MicrosoftIdentityAuthenticationSettings settings)
    {
        authBuilder.AddMicrosoftIdentityWebApi(o =>
            o.Events = new JwtBearerEvents
            {
                OnTokenValidated = OnTokenValidated,
                OnChallenge = OnChallenge
            }, o =>
        {
            o.Instance = settings.Instance;
            o.Domain = settings.Domain;
            o.SignUpSignInPolicyId = settings.SignUpSignInPolicyId;
            o.TenantId = settings.TenantId;
            o.ClientId = settings.ClientId;
            o.ClientSecret = settings.ClientSecret;
        });

        return authBuilder;
    }

    public static IServiceCollection AddAuthenticationServices(this IServiceCollection services)
    {
        services
            .AddScoped<FindUserId>()
            .AddScoped<AppAuthenticationHandler>();

        return services;
    }

    private static async Task OnTokenValidated(TokenValidatedContext context)
    {
        context.HttpContext.User = context.Principal!;

        var authHandler = context.HttpContext.RequestServices.GetRequiredService<AppAuthenticationHandler>();

        await authHandler.HandleAsync(context);
    }

    private static Task OnChallenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AppAuthenticationHandler>>();

        var authException = context.AuthenticateFailure;
        if (authException is not null)
            logger.LogCritical(authException, "Error while authenticating user.");

        var responseBody = ErrorResponse.Create(ChangeBlogStrings.NotAuthenticated);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = MediaTypeNames.Application.Json;
        return context.Response.WriteAsync(JsonSerializer.Serialize(responseBody,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            }));

        // context.Response.OnStarting(async () =>
        // {
        //     var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<AppAuthenticationHandler>>();
        //
        //     var authException = context.AuthenticateFailure;
        //     if (authException is not null)
        //         logger.LogCritical(authException, "Error while authenticating user.");
        //
        //     context.Response.ContentType = MediaTypeNames.Application.Json;
        //     var responseBody = ErrorResponse.Create(ChangeBlogStrings.NotAuthenticated);
        //
        //     await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody,
        //         new JsonSerializerOptions
        //         {
        //             PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        //         }));
        // });
    }
}