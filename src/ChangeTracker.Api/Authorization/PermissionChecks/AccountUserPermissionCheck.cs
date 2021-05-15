using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
using ChangeTracker.Domain;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    /// <summary>
    ///     Checks if the user has any permission within its accounts
    /// </summary>
    public class AccountUserPermissionCheck : PermissionCheck
    {
        private readonly UserAccessDao _userAccessDao;

        public AccountUserPermissionCheck(UserAccessDao userAccessDao)
        {
            _userAccessDao = userAccessDao;
        }

        public override Task<bool> HasPermission(ActionExecutingContext context, Guid userId, Permission permission)
            => _userAccessDao.HasUserPermissionAsync(userId, permission);
    }
}