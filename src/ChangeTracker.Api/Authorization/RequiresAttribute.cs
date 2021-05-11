using System;
using ChangeTracker.Application.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChangeTracker.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class RequiresAttribute : Attribute
    {
        public Permission Permission { get; }

        public RequiresAttribute(Permission permission)
        {
            Permission = permission;
        }
    }
}