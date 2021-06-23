using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.GetAuthorizationState;
using ChangeTracker.Domain.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.AuthorizationHandlers
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