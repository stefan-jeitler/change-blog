using System;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.Authorization.RequestBodyIdentifiers;
using ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace ChangeBlog.Api.Shared.Authorization.AuthorizationHandlers;

public class ProductAuthorizationHandler : AuthorizationHandler
{
    private readonly AuthorizationHandler _authorizationHandler;
    private readonly IGetAuthorizationState _getAuthorizationState;

    public ProductAuthorizationHandler(AuthorizationHandler authorizationHandlerComponent,
        IGetAuthorizationState getAuthorizationState)
    {
        _authorizationHandler = authorizationHandlerComponent;
        _getAuthorizationState = getAuthorizationState;
    }

    public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
        Permission permission)
    {
        var productIdInRoute = TryFindIdInRoute(context.HttpContext, KnownIdentifiers.ProductId);
        if (productIdInRoute.HasValue)
        {
            return _getAuthorizationState.GetAuthStateByProductIdAsync(userId, productIdInRoute.Value, permission);
        }

        var productIdInBody = TryFindInBody<IContainsProductId>(context);
        if (productIdInBody is not null)
        {
            return _getAuthorizationState.GetAuthStateByProductIdAsync(userId, productIdInBody.ProductId,
                permission);
        }

        return _authorizationHandler.GetAuthorizationState(context, userId, permission);
    }
}