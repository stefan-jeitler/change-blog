using System;
using System.Collections.Generic;

namespace ChangeBlog.Management.Api.DTOs.Permissions;

public record ResourcePermissionsDto
{
    public Guid ResourceId { get; set; }
    public ResourceType ResourceType { get; set; }

    public bool CanRead { get; set; }
    public bool CanUpdate { get; set; }
    public bool CanDelete { get; set; }
    public IReadOnlyDictionary<string, bool> SpecificPermissions { get; set; }
}