using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace ChangeBlog.Management.Api.Authentication
{
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
                    o.TenantId = settings.TenantId;
                    o.ClientId = settings.ClientId;
                    o.ClientSecret = settings.ClientSecret;
                })
                .EnableTokenAcquisitionToCallDownstreamApi(_ => { })
                .AddInMemoryTokenCaches();

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
            var authHandler = context.HttpContext.RequestServices.GetRequiredService<AppAuthenticationHandler>();

            await authHandler.HandleAsync(context);
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
}