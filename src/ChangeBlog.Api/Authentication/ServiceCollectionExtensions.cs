using System.Linq;
using ChangeBlog.Api.Shared.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace ChangeBlog.Api.Authentication;

public static class ServiceCollectionExtensions
{
    private const string PolicyScheme = "SchemePicker";
    private const string ApiKeyScheme = ApiKeyAuthenticationOptions.Scheme;
    private const string AzureAdB2C = "Azure AD B2C";

    public static IServiceCollection AddAppAuthentication(this IServiceCollection services,
        MicrosoftIdentityAuthenticationSettings settings)
    {
        services.AddScoped<FindUserId>();
        services.AddAuthentication(PolicyScheme)
            .AddPolicyScheme(PolicyScheme, "Authorization - JWT or ApiKey", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                    return authHeader is not null && authHeader.StartsWith("Bearer ")
                        ? AzureAdB2C
                        : ApiKeyScheme;
                };
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                ApiKeyScheme, _ => { })
            .AddMicrosoftIdentityWebApi(o =>
            {
                o.Events = new JwtBearerEvents
                {
                    OnTokenValidated = JwtAuthUtils.IssueAppTicket,
                    OnChallenge = JwtAuthUtils.Challenge
                };
            }, o =>
            {
                o.Instance = settings.Instance;
                o.TenantId = settings.TenantId;
                o.Domain = settings.Domain;
                o.SignUpSignInPolicyId = settings.SignUpSignInPolicyId;
                o.ClientId = settings.ClientId;
                o.ClientSecret = settings.ClientSecret;
            }, AzureAdB2C);

        return services;
    }
}