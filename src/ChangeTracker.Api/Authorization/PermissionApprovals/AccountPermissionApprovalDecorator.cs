using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable ClassNeverInstantiated.Global

namespace ChangeTracker.Api.Authorization.PermissionApprovals
{
    public class AccountPermissionApprovalDecorator : PermissionApproval
    {
        private readonly PermissionApproval _permissionApprovalComponent;
        private readonly UserAccessDao _userAccessDao;

        public AccountPermissionApprovalDecorator(PermissionApproval permissionApprovalComponent, UserAccessDao userAccessDao)
        {
            _permissionApprovalComponent = permissionApprovalComponent;
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

            return await _permissionApprovalComponent.HasPermission(context, userId, permission);
        }
    }
}