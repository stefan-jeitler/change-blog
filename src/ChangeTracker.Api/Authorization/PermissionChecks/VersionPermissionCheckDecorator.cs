using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    public class VersionPermissionCheckDecorator : PermissionCheck
    {
        private readonly PermissionCheck _permissionCheckComponent;
        private readonly UserAccessDao _userAccessDao;

        public VersionPermissionCheckDecorator(PermissionCheck permissionCheckComponent, UserAccessDao userAccessDao)
        {
            _permissionCheckComponent = permissionCheckComponent;
            _userAccessDao = userAccessDao;
        }

        public override async Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission)
        {
            var versionId = TryFindIdInRouteValues(httpContext, "versionId");
            if (versionId.HasValue)
            {
                return await _userAccessDao.HasVersionPermissionAsync(userId, versionId.Value, permission);
            }

            return await _permissionCheckComponent.HasPermission(httpContext, userId, permission);
        }
    }
}