using System;
using System.Threading.Tasks;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeBlog.Api.Shared.Authorization.AuthorizationHandlers;

public class UnauthorizedHandler : AuthorizationHandler
{
    public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
        Permission permission)
    {
        return Task.FromResult(AuthorizationState.Unauthorized);
    }
}