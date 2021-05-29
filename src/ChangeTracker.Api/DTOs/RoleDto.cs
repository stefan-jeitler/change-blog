﻿using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.Queries.GetRoles;

namespace ChangeTracker.Api.DTOs
{
    public class RoleDto
    {
        public RoleDto(string name, IList<string> permissions)
        {
            Name = name;
            Permissions = permissions;
        }

        public string Name { get; }
        public IList<string> Permissions { get; }

        public static RoleDto FromResponseModel(RoleResponseModel m, bool includePermission) =>
            includePermission
                ? new RoleDto(m.Name, m.Permissions)
                : new RoleDto(m.Name, Array.Empty<string>());
    }
}