using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionApprovals
{
    /// <summary>
    ///     Check if the user has any permission within its accounts
    /// </summary>
    public class AccountUserPermissionApproval : PermissionApproval
    {
        private readonly UserAccessDao _userAccessDao;

        public AccountUserPermissionApproval(UserAccessDao userAccessDao)
        {
            _userAccessDao = userAccessDao;
        }

        public override Task<bool> HasPermission(ActionExecutingContext context, Guid userId, Permission permission)
            => _userAccessDao.HasUserPermissionAsync(userId, permission);
    }
}