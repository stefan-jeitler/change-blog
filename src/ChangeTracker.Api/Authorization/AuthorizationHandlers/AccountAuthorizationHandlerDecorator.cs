using System;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;
using ChangeTracker.Application.UseCases.Queries.GetAuthorizationState;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable ClassNeverInstantiated.Global

namespace ChangeTracker.Api.Authorization.AuthorizationHandlers
{
    public class AccountAuthorizationHandlerDecorator : AuthorizationHandler
    {
        private readonly AuthorizationHandler _authorizationHandlerComponent;
        private readonly IGetAuthorizationState _getAuthorizationState;

        public AccountAuthorizationHandlerDecorator(AuthorizationHandler authorizationHandlerComponent, IGetAuthorizationState getAuthorizationState)
        {
            _authorizationHandlerComponent = authorizationHandlerComponent;
            _getAuthorizationState = getAuthorizationState;
        }

        public override async Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var accountIdInRoute = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.AccountId);
            if (accountIdInRoute.HasValue)
                return await _getAuthorizationState.GetAuthStateByAccountIdAsync(userId, accountIdInRoute.Value, permission);

            var accountIdInBody = TryFindInBody<IContainsAccountId>(context);
            if (accountIdInBody is not null)
                return await _getAuthorizationState.GetAuthStateByAccountIdAsync(userId, accountIdInBody.AccountId, permission);

            return await _authorizationHandlerComponent.GetAuthorizationState(context, userId, permission);
        }
    }
}