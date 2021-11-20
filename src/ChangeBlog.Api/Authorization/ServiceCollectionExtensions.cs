using ChangeBlog.Api.Authorization.AuthorizationHandlers;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeBlog.Api.Authorization;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPermissionHandler(this IServiceCollection services)
    {
        return services.AddScoped<AuthorizationHandler, UnauthorizedHandler>()
            .Decorate<AuthorizationHandler, AccountAuthorizationHandler>()
            .Decorate<AuthorizationHandler, ProductAuthorizationHandler>()
            .Decorate<AuthorizationHandler, VersionAuthorizationHandler>()
            .Decorate<AuthorizationHandler, ChangeLogLineAuthorizationHandler>();
    }
}