using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionApprovals
{
    public class ChangeLogLinePermissionApprovalDecorator : PermissionApproval
    {
        private readonly PermissionApproval _permissionApprovalComponent;
        private readonly UserAccessDao _userAccessDao;

        public ChangeLogLinePermissionApprovalDecorator(PermissionApproval permissionApprovalComponent,
            UserAccessDao userAccessDao)
        {
            _permissionApprovalComponent = permissionApprovalComponent;
            _userAccessDao = userAccessDao;
        }

        public override async Task<bool> HasPermission(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var changeLogLineId = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.ChangeLogLineId);
            if (changeLogLineId.HasValue)
                return await _userAccessDao.HasChangeLogLinePermissionAsync(userId, changeLogLineId.Value, permission);

            return await _permissionApprovalComponent.HasPermission(context, userId, permission);
        }
    }
}