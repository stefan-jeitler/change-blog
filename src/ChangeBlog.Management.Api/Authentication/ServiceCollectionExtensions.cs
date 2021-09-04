using System;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.DTOs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;

namespace ChangeBlog.Management.Api.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static AuthenticationBuilder AddMicrosoftIdentityAuthentication(this AuthenticationBuilder authBuilder,
            MicrosoftIdentityAuthenticationSettings settings)
        {
            authBuilder.AddMicrosoftIdentityWebApi(o => { o.Events = CreateCustomChallengeHandler(); }, o =>
                {
                    o.Instance = settings.Instance;
                    o.TenantId = settings.TenantId;
                    o.ClientId = settings.ClientId;
                    o.ClientSecret = settings.ClientSecret;
                })
                .EnableTokenAcquisitionToCallDownstreamApi(o => { })
                .AddInMemoryTokenCaches();

            return authBuilder;
        }


        private static JwtBearerEvents CreateCustomChallengeHandler()
        {
            return new()
            {
                OnChallenge = context =>
                {
                    context.Response.OnStarting(async () =>
                    {
                        context.Response.ContentType = MediaTypeNames.Application.Json;
                        var responseBody = DefaultResponse.Create("You are not authenticated. Please add a valid JWT Bearer Token.");

                        await context.Response.WriteAsync(JsonSerializer.Serialize(responseBody,
                            new JsonSerializerOptions
                            {
                                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                            }));
                    });

                    return Task.CompletedTask;
                }
            };
        }
    }
}