using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Application.UseCases.Queries.GetRoles;

namespace ChangeTracker.Api.DTOs
{
    public class RoleDto
    {
        public string Name { get; set; }
        public List<string> Permissions { get; set; }

        public static RoleDto FromResponseModel(RoleResponseModel m, bool includePermission)
        {
            var response = new RoleDto
            {
                Name = m.Name,
            };

            if (includePermission)
            {
                response.Permissions = m.Permissions.ToList();
            }

            return response;
        }
    }
}