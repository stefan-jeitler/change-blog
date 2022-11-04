using System;
using System.Collections.Generic;

namespace ChangeBlog.Management.Api.DTOs.Permissions;

public record ResourcePermissionsDto
{
    public Guid ResourceId { get; init; }
    public ResourceType ResourceType { get; init; }

    public bool CanRead { get; init; }
    public bool CanUpdate { get; init; }
    public bool CanDelete { get; init; }
    public IReadOnlyDictionary<string, bool> SpecificPermissions { get; init; }
}