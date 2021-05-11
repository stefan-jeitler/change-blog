using ChangeTracker.Api.Authorization.PermissionChecks;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services) =>
            services.AddScoped<IPermissionCheck, NotAuthorized>()
                .Decorate<IPermissionCheck, AccountPermissionCheckDecorator>()
                .Decorate<IPermissionCheck, ProjectPermissionCheckDecorator>()
                .Decorate<IPermissionCheck, VersionPermissionCheckDecorator>()
                .Decorate<IPermissionCheck, ChangeLogLinePermissionCheckDecorator>();
    }
}