using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Account;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.User;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable ClassNeverInstantiated.Global

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

        public override async Task<bool> HasPermission(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var accountIdInRoute = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.AccountId);
            if (accountIdInRoute.HasValue)
                return await _userAccessDao.HasAccountPermissionAsync(userId, accountIdInRoute.Value, permission);

            var accountIdInBody = TryFindInBody<IContainsAccountId>(context);
            if (accountIdInBody is not null)
                return await _userAccessDao.HasAccountPermissionAsync(userId, accountIdInBody.AccountId, permission);

            return await _permissionCheckComponent.HasPermission(context, userId, permission);
        }
    }
}