using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.User;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    public class ProductPermissionCheckDecorator : PermissionCheck
    {
        private readonly PermissionCheck _permissionCheckComponent;
        private readonly UserAccessDao _userAccessDao;

        public ProductPermissionCheckDecorator(PermissionCheck permissionCheckComponent, UserAccessDao userAccessDao)
        {
            _permissionCheckComponent = permissionCheckComponent;
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

            return await _permissionCheckComponent.HasPermission(context, userId, permission);
        }
    }
}