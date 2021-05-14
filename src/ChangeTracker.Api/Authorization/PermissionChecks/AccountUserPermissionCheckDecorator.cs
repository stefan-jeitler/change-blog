﻿using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres;
using ChangeTracker.Domain;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    /// <summary>
    /// Checks if the user has any permission within its accounts
    /// </summary>
    public class AccountUserPermissionCheckDecorator : PermissionCheck
    {
        private readonly UserAccessDao _userAccessDao;

        public AccountUserPermissionCheckDecorator(UserAccessDao userAccessDao)
        {
            _userAccessDao = userAccessDao;
        }

        public override Task<bool> HasPermission(ActionExecutingContext context, Guid userId, Permission permission) 
            => _userAccessDao.HasUserPermissionAsync(userId, permission);
    }
}
