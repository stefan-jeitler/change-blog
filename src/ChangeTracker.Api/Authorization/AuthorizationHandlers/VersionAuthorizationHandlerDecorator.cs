using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.GetAuthorizationState;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.AuthorizationHandlers
{
    public class VersionAuthorizationHandlerDecorator : AuthorizationHandler
    {
        private readonly AuthorizationHandler _authorizationHandlerComponent;
        private readonly IGetAuthorizationState _getAuthorizationState;

        public VersionAuthorizationHandlerDecorator(AuthorizationHandler authorizationHandlerComponent, IGetAuthorizationState getAuthorizationState)
        {
            _authorizationHandlerComponent = authorizationHandlerComponent;
            _getAuthorizationState = getAuthorizationState;
        }

        public override async Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var versionId = TryFindIdInHeader(context.HttpContext, KnownIdentifiers.VersionId);
            if (versionId.HasValue)
                return await _getAuthorizationState.GetAuthStateByVersionIdAsync(userId, versionId.Value, permission);

            return await _authorizationHandlerComponent.GetAuthorizationState(context, userId, permission);
        }
    }
}