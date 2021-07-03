using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.GetAuthorizationState;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.AuthorizationHandlers
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