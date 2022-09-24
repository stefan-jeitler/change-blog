using System;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Accounts.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeBlog.Api.Shared.Authorization.AuthorizationHandlers;

[UsedImplicitly]
public class ChangeLogLineAuthorizationHandler : AuthorizationHandler
{
    private readonly AuthorizationHandler _authorizationHandler;
    private readonly IGetAuthorizationState _getAuthorizationState;

    public ChangeLogLineAuthorizationHandler(AuthorizationHandler authorizationHandlerComponent,
        IGetAuthorizationState getAuthorizationState)
    {
        _authorizationHandler = authorizationHandlerComponent;
        _getAuthorizationState = getAuthorizationState;
    }

    public override Task<AuthorizationState> GetAuthorizationStateAsync(ActionExecutingContext context, Guid userId,
        Permission permission)
    {
        var changeLogLineId = TryFindIdInRoute(context.HttpContext, KnownIdentifiers.ChangeLogLineId);

        return changeLogLineId.HasValue
            ? _getAuthorizationState.GetAuthStateByChangeLogLineIdAsync(userId, changeLogLineId.Value, permission)
            : _authorizationHandler.GetAuthorizationStateAsync(context, userId, permission);
    }
}