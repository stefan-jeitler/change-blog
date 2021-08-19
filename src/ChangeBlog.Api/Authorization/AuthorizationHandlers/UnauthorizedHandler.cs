using System;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeBlog.Api.Authorization.AuthorizationHandlers
{
    public class UnauthorizedHandler : AuthorizationHandler
    {
        public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
            => Task.FromResult(AuthorizationState.Unauthorized);
    }
}