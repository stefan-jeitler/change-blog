﻿using System;
using ChangeTracker.Application.UseCases;

namespace ChangeTracker.Api.Authorization
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
    public class NeedsPermissionAttribute : Attribute
    {
        public NeedsPermissionAttribute(Permission permission)
        {
            Permission = permission;
        }

        public Permission Permission { get; }
    }
}