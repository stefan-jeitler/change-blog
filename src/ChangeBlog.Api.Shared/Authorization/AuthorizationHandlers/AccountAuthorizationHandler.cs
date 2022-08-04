using System;
using System.Threading.Tasks;
using ChangeBlog.Api.Shared.Authorization.RequestBodyIdentifiers;
using ChangeBlog.Application.UseCases.Accounts.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable ConvertIfStatementToReturnStatement

// ReSharper disable ClassNeverInstantiated.Global

namespace ChangeBlog.Api.Shared.Authorization.AuthorizationHandlers;

public class AccountAuthorizationHandler : AuthorizationHandler
{
    private readonly AuthorizationHandler _authorizationHandler;
    private readonly IGetAuthorizationState _getAuthorizationState;

    public AccountAuthorizationHandler(AuthorizationHandler authorizationHandlerComponent,
        IGetAuthorizationState getAuthorizationState)
    {
        _authorizationHandler = authorizationHandlerComponent;
        _getAuthorizationState = getAuthorizationState;
    }

    public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
        Permission permission)
    {
        var accountIdInRoute = TryFindIdInRoute(context.HttpContext, KnownIdentifiers.AccountId);
        if (accountIdInRoute.HasValue)
        {
            return _getAuthorizationState.GetAuthStateByAccountIdAsync(userId, accountIdInRoute.Value, permission);
        }

        var accountIdInBody = TryFindInBody<IContainsAccountId>(context);
        if (accountIdInBody is not null)
        {
            return _getAuthorizationState.GetAuthStateByAccountIdAsync(userId, accountIdInBody.AccountId,
                permission);
        }

        return _authorizationHandler.GetAuthorizationState(context, userId, permission);
    }
}