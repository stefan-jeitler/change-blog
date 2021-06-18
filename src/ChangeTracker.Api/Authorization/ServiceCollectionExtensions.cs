using ChangeTracker.Api.Authorization.PermissionApprovals;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Authorization
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPermissionCheck(this IServiceCollection services) =>
            services.AddScoped<PermissionApproval, AccountUserPermissionApproval>()
                .Decorate<PermissionApproval, AccountPermissionApprovalDecorator>()
                .Decorate<PermissionApproval, ProductPermissionApprovalDecorator>()
                .Decorate<PermissionApproval, VersionPermissionApprovalDecorator>()
                .Decorate<PermissionApproval, ChangeLogLinePermissionApprovalDecorator>();
    }
}