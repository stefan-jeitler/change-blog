using ChangeBlog.Api.Authentication.DataAccess;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeBlog.Api.Authentication
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApiKeyAuthentication(this IServiceCollection services)
        {
            services.AddScoped<IFindUserId, FindUserIdAdapter>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
                })
                .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
                    ApiKeyAuthenticationOptions.DefaultScheme, _ => { });

            return services;
        }
    }
}