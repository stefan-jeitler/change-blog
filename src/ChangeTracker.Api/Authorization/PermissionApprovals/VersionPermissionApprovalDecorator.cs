using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionApprovals
{
    public class VersionPermissionApprovalDecorator : PermissionApproval
    {
        private readonly PermissionApproval _permissionApprovalComponent;
        private readonly UserAccessDao _userAccessDao;

        public VersionPermissionApprovalDecorator(PermissionApproval permissionApprovalComponent, UserAccessDao userAccessDao)
        {
            _permissionApprovalComponent = permissionApprovalComponent;
            _userAccessDao = userAccessDao;
        }

        public override async Task<bool> HasPermission(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var versionId = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.VersionId);
            if (versionId.HasValue)
                return await _userAccessDao.HasVersionPermissionAsync(userId, versionId.Value, permission);

            return await _permissionApprovalComponent.HasPermission(context, userId, permission);
        }
    }
}