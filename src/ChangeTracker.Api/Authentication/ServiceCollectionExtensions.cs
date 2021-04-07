using ChangeTracker.Api.Authentication.AccountDaos;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services,
            bool useInMemoryApiKeys)
        {
            if (useInMemoryApiKeys)
            {
                services
                    .AddSingleton<IAccountDao, InMemoryAccountDao>(
                        _ => new InMemoryAccountDao("test-api-key"));
            }
            else
            {
                // TODO: replace it with production api keys stored in db
                services.AddScoped<IAccountDao, InMemoryAccountDao>(_ => new InMemoryAccountDao("test-api-key"));
            }

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenticationOptions.DefaultScheme, o => { });

            return services;
        }
    }
}