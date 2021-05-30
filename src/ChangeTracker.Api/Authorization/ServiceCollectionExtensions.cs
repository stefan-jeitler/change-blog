using ChangeTracker.Api.Authorization.PermissionChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissionCheck(this IServiceCollection services) =>
            services.AddScoped<PermissionCheck, AccountUserPermissionCheck>()
                .Decorate<PermissionCheck, AccountPermissionCheckDecorator>()
                .Decorate<PermissionCheck, ProductPermissionCheckDecorator>()
                .Decorate<PermissionCheck, VersionPermissionCheckDecorator>()
                .Decorate<PermissionCheck, ChangeLogLinePermissionCheckDecorator>();
    }
}