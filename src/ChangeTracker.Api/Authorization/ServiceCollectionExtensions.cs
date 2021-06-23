using ChangeTracker.Api.Authorization.AuthorizationHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissionCheck(this IServiceCollection services) =>
            services.AddScoped<AuthorizationHandler, UserAccountsAuthorizationHandler>()
                .Decorate<AuthorizationHandler, AccountAuthorizationHandlerDecorator>()
                .Decorate<AuthorizationHandler, ProductAuthorizationHandlerDecorator>()
                .Decorate<AuthorizationHandler, VersionAuthorizationHandlerDecorator>()
                .Decorate<AuthorizationHandler, ChangeLogLineAuthorizationHandlerDecorator>();
    }
}