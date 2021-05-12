using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    public class ChangeLogLinePermissionCheckDecorator : PermissionCheck
    {
        private readonly PermissionCheck _permissionCheckComponent;
        private readonly UserAccessDao _userAccessDao;

        public ChangeLogLinePermissionCheckDecorator(PermissionCheck permissionCheckComponent, UserAccessDao userAccessDao)
        {
            _permissionCheckComponent = permissionCheckComponent;
            _userAccessDao = userAccessDao;
        }

        public override async Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission)
        {
            var changeLogLineId = TryFindIdInRouteValues(httpContext, "changeLogLineId");
            if (changeLogLineId.HasValue)
            {
                return await _userAccessDao.HasChangeLogLinePermissionAsync(userId, changeLogLineId.Value, permission);
            }

            return await _permissionCheckComponent.HasPermission(httpContext, userId, permission);
        }
    }
}
