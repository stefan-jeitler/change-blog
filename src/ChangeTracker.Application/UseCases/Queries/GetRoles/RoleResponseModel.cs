using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Queries.GetRoles
{
    public class RoleResponseModel
    {
        public RoleResponseModel(string name, IList<string> permissions)
        {
            Name = name;
            Permissions = permissions;
        }

        public string Name { get; }
        public IList<string> Permissions { get; }
    }
}