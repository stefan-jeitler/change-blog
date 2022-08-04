using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.Accounts.GetRoles;

namespace ChangeBlog.Api.DTOs;

public class RoleDto
{
    public RoleDto(string name, IList<string> permissions)
    {
        Name = name;
        Permissions = permissions;
    }

    public string Name { get; }
    public IList<string> Permissions { get; }

    public static RoleDto FromResponseModel(RoleResponseModel m, bool includePermission)
    {
        return includePermission
            ? new RoleDto(m.Name, m.Permissions)
            : new RoleDto(m.Name, Array.Empty<string>());
    }
}