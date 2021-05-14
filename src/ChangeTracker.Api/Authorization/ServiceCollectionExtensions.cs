using ChangeTracker.Api.Authorization.PermissionChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissionCheck(this IServiceCollection services) =>
            services.AddScoped<PermissionCheck, AccountUserPermissionCheckDecorator>()
                .Decorate<PermissionCheck, ChangeLogLinePermissionCheckDecorator>()
                .Decorate<PermissionCheck, VersionPermissionCheckDecorator>()
                .Decorate<PermissionCheck, ProjectPermissionCheckDecorator>()
                .Decorate<PermissionCheck, AccountPermissionCheckDecorator>();
    }
}