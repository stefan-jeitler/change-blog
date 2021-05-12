using System;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class NeedsPermissionAttribute : Attribute
    {
        public Permission Permission { get; }

        public NeedsPermissionAttribute(Permission permission)
        {
            Permission = permission;
        }
    }
}