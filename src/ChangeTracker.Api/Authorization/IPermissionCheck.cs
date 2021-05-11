using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Http;

namespace ChangeTracker.Api.Authorization
{
    public interface IPermissionCheck
    {
        Task<bool> HasPermission(HttpContext httpContext, Guid userId, Permission permission);
    }
}
