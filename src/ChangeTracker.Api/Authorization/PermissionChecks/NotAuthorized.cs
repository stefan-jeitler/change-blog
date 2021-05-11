using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Authorization.PermissionChecks
{
    public class NotAuthorized : IPermissionCheck
    {
        public Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission) =>
            Task.FromResult(false);
    }
}