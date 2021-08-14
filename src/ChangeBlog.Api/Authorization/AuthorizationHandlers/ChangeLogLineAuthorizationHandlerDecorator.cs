using System;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeBlog.Api.Authorization.AuthorizationHandlers
{
    public class ChangeLogLineAuthorizationHandlerDecorator : AuthorizationHandler
    {
        private readonly AuthorizationHandler _authorizationHandlerComponent;
        private readonly IGetAuthorizationState _getAuthorizationState;

        public ChangeLogLineAuthorizationHandlerDecorator(AuthorizationHandler authorizationHandlerComponent, IGetAuthorizationState getAuthorizationState)
        {
            _authorizationHandlerComponent = authorizationHandlerComponent;
            _getAuthorizationState = getAuthorizationState;
        }

        public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var changeLogLineId = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.ChangeLogLineId);

            return changeLogLineId.HasValue 
                ? _getAuthorizationState.GetAuthStateByChangeLogLineIdAsync(userId, changeLogLineId.Value, permission) 
                : _authorizationHandlerComponent.GetAuthorizationState(context, userId, permission);
        }
    }
}
