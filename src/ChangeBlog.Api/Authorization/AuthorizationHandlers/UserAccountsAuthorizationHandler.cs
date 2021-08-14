using System;
using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Queries.GetAuthorizationState;
using ChangeBlog.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeBlog.Api.Authorization.AuthorizationHandlers
{
    /// <summary>
    ///     Check if the user has any permission within its accounts
    /// </summary>
    public class UserAccountsAuthorizationHandler : AuthorizationHandler
    {
        private readonly IGetAuthorizationState _getAuthorizationState;

        public UserAccountsAuthorizationHandler(IGetAuthorizationState getAuthorizationState)
        {
            _getAuthorizationState = getAuthorizationState;
        }

        public override Task<AuthorizationState> GetAuthorizationState(ActionExecutingContext context, Guid userId,
            Permission permission)
            => _getAuthorizationState.GetAuthStateByUserAccountsAsync(userId, permission);
    }
}
