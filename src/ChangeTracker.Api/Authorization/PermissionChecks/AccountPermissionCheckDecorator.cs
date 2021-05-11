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
    public class AccountPermissionCheckDecorator : IPermissionCheck
    {
        private readonly IPermissionCheck _permissionCheckComponent;
        private readonly UserDao _userDao;

        public AccountPermissionCheckDecorator(IPermissionCheck permissionCheckComponent, UserDao userDao)
        {
            _permissionCheckComponent = permissionCheckComponent;
            _userDao = userDao;
        }

        public async Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission)
        {
            var accountId = ExtractAccountId(httpContext);
            if (accountId.HasValue)
            {
                return await _userDao.HasAccountPermission(userId, accountId.Value, permission);
            }
            
            return await _permissionCheckComponent.HasPermission(httpContext, userId, permission);
        }

        private Guid? ExtractAccountId(HttpContext httpContext)
        {
            return Guid.Parse("8b41a1d3-5f56-4b76-bb4f-17a8bc304e7f");
        }
    }
}
