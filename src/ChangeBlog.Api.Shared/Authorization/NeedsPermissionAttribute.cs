using System;
using ChangeBlog.Domain.Authorization;

namespace ChangeBlog.Api.Shared.Authorization;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false)]
public class NeedsPermissionAttribute : Attribute
{
    public NeedsPermissionAttribute(Permission permission)
    {
        Permission = permission;
    }

    public Permission Permission { get; }
}