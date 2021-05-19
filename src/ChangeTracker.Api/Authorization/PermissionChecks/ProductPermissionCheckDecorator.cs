using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
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
            var productId = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.ProductId);
            if (productId.HasValue)
                return await _userAccessDao.HasProductPermissionAsync(userId, productId.Value, permission);

            return await _permissionCheckComponent.HasPermission(context, userId, permission);
        }
    }
}