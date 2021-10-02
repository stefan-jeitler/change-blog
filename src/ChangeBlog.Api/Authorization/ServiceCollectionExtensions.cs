using ChangeBlog.Api.Authorization.AuthorizationHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeBlog.Api.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissionCheck(this IServiceCollection services) =>
            services.AddScoped<AuthorizationHandler, UnauthorizedHandler>()
                .Decorate<AuthorizationHandler, AccountAuthorizationHandler>()
                .Decorate<AuthorizationHandler, ProductAuthorizationHandler>()
                .Decorate<AuthorizationHandler, VersionAuthorizationHandler>()
                .Decorate<AuthorizationHandler, ChangeLogLineAuthorizationHandler>();
    }
}