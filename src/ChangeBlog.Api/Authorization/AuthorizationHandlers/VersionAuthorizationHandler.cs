using System;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeBlog.Api.Authorization.AuthorizationHandlers
{
    public class VersionAuthorizationHandler : AuthorizationHandler
    {
        private readonly AuthorizationHandler _authorizationHandler;
        private readonly IGetAuthorizationState _getAuthorizationState;

        public VersionAuthorizationHandler(AuthorizationHandler authorizationHandlerComponent,
            IGetAuthorizationState getAuthorizationState)
        {
            _authorizationHandler = authorizationHandlerComponent;
            _getAuthorizationState = getAuthorizationState;
        }

        public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
        {
            var versionId = TryFindIdInRoute(context.HttpContext, KnownIdentifiers.VersionId);

            return versionId.HasValue
                ? _getAuthorizationState.GetAuthStateByVersionIdAsync(userId, versionId.Value, permission)
                : _authorizationHandler.GetAuthorizationState(context, userId, permission);
        }
    }
}