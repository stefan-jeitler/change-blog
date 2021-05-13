﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    public class NoPermission : PermissionCheck
    {
        public override Task<bool> HasPermission(ActionExecutingContext context, Guid userId, Permission permission) =>
            Task.FromResult(false);
    }
}