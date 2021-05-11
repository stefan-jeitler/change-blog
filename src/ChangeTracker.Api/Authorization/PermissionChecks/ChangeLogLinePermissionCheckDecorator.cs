using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    public class ChangeLogLinePermissionCheckDecorator : IPermissionCheck
    {
        private readonly IPermissionCheck _permissionCheckComponent;

        public ChangeLogLinePermissionCheckDecorator(IPermissionCheck permissionCheckComponent)
        {
            _permissionCheckComponent = permissionCheckComponent;
        }

        public Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission)
            => _permissionCheckComponent.HasPermission(httpContext, userId, permission);
    }
}
