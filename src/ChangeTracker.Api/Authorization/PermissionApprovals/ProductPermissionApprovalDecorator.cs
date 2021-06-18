using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionApprovals
{
    public class ProductPermissionApprovalDecorator : PermissionApproval
    {
        private readonly PermissionApproval _permissionApprovalComponent;
        private readonly UserAccessDao _userAccessDao;

        public ProductPermissionApprovalDecorator(PermissionApproval permissionApprovalComponent, UserAccessDao userAccessDao)
        {
            _permissionApprovalComponent = permissionApprovalComponent;
            _userAccessDao = userAccessDao;
        }

        public override async Task<bool> HasPermission(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var productIdInRoute = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.ProductId);
            if (productIdInRoute.HasValue)
                return await _userAccessDao.HasProductPermissionAsync(userId, productIdInRoute.Value, permission);

            var productIdInBody = TryFindInBody<IContainsProductId>(context);
            if (productIdInBody is not null)
                return await _userAccessDao.HasProductPermissionAsync(userId, productIdInBody.ProductId, permission);

            return await _permissionApprovalComponent.HasPermission(context, userId, permission);
        }
    }
}