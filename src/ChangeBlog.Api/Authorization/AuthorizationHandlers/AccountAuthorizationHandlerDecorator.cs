using System;
using System.Threading.Tasks;
using ChangeBlog.Api.Authorization.RequestBodyIdentifiers;
using ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

// ReSharper disable ConvertIfStatementToReturnStatement

// ReSharper disable ClassNeverInstantiated.Global

namespace ChangeBlog.Api.Authorization.AuthorizationHandlers
{
    public class AccountAuthorizationHandlerDecorator : AuthorizationHandler
    {
        private readonly AuthorizationHandler _authorizationHandlerComponent;
        private readonly IGetAuthorizationState _getAuthorizationState;

        public AccountAuthorizationHandlerDecorator(AuthorizationHandler authorizationHandlerComponent,
            IGetAuthorizationState getAuthorizationState)
        {
            _authorizationHandlerComponent = authorizationHandlerComponent;
            _getAuthorizationState = getAuthorizationState;
        }

        public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var accountIdInRoute = TryFindIdInRoute(context.HttpContext, KnownIdentifiers.AccountId);
            if (accountIdInRoute.HasValue)
                return _getAuthorizationState.GetAuthStateByAccountIdAsync(userId, accountIdInRoute.Value, permission);

            var accountIdInBody = TryFindInBody<IContainsAccountId>(context);
            if (accountIdInBody is not null)
                return _getAuthorizationState.GetAuthStateByAccountIdAsync(userId, accountIdInBody.AccountId,
                    permission);

            return _authorizationHandlerComponent.GetAuthorizationState(context, userId, permission);
        }
    }
}