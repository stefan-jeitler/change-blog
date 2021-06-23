using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases.Queries.GetAuthorizationState;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.AuthorizationHandlers
{
    public class ProductAuthorizationHandlerDecorator : AuthorizationHandler
    {
        private readonly AuthorizationHandler _authorizationHandlerComponent;
        private readonly IGetAuthorizationState _getAuthorizationState;

        public ProductAuthorizationHandlerDecorator(AuthorizationHandler authorizationHandlerComponent, IGetAuthorizationState getAuthorizationState)
        {
            _authorizationHandlerComponent = authorizationHandlerComponent;
            _getAuthorizationState = getAuthorizationState;
        }

        public override async Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var productIdInRoute = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.ProductId);
            if (productIdInRoute.HasValue)
                return await _getAuthorizationState.GetAuthStateByProductIdAsync(userId, productIdInRoute.Value, permission);

            var productIdInBody = TryFindInBody<IContainsProductId>(context);
            if (productIdInBody is not null)
                return await _getAuthorizationState.GetAuthStateByProductIdAsync(userId, productIdInBody.ProductId, permission);

            return await _authorizationHandlerComponent.GetAuthorizationState(context, userId, permission);
        }
    }
}