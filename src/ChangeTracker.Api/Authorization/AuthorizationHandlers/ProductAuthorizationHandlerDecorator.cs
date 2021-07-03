using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases.Queries.GetAuthorizationState;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
// ReSharper disable ConvertIfStatementToReturnStatement

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

        public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var productIdInRoute = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.ProductId);
            if (productIdInRoute.HasValue)
                return _getAuthorizationState.GetAuthStateByProductIdAsync(userId, productIdInRoute.Value, permission);

            var productIdInBody = TryFindInBody<IContainsProductId>(context);
            if (productIdInBody is not null)
                return _getAuthorizationState.GetAuthStateByProductIdAsync(userId, productIdInBody.ProductId, permission);

            return _authorizationHandlerComponent.GetAuthorizationState(context, userId, permission);
        }
    }
}