using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    public class AccountPermissionCheckDecorator : PermissionCheck
    {
        private readonly PermissionCheck _permissionCheckComponent;
        private readonly UserAccessDao _userAccessDao;

        public AccountPermissionCheckDecorator(PermissionCheck permissionCheckComponent, UserAccessDao userAccessDao)
        {
            _permissionCheckComponent = permissionCheckComponent;
            _userAccessDao = userAccessDao;
        }

        public override async Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission)
        {
            var accountId = TryFindIdInRouteValues(httpContext, "accountId");
            if (accountId.HasValue)
            {
                return await _userAccessDao.HasAccountPermissionAsync(userId, accountId.Value, permission);
            }
            
            return await _permissionCheckComponent.HasPermission(httpContext, userId, permission);
        }
    }
}
