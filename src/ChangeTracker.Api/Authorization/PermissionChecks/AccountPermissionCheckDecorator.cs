using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres;
using ChangeTracker.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

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

        public override async Task<bool> HasPermission(ActionExecutingContext context, Guid userId, Permission permission)
        {
            var accountIdInRoute = TryFindIdInHeader(context.HttpContext, "accountId");
            if (accountIdInRoute.HasValue)
            {
                return await _userAccessDao.HasAccountPermissionAsync(userId, accountIdInRoute.Value, permission);
            }

            var accountIdInBody = TryFindInBody<IContainsAccountId>(context);
            if (accountIdInBody != null)
            {
                return await _userAccessDao.HasAccountPermissionAsync(userId, accountIdInBody.AccountId, permission);
            }

            return await _permissionCheckComponent.HasPermission(context, userId, permission);
        }
    }
}