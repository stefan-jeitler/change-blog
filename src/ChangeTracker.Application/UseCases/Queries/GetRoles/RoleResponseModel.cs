using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Queries.GetRoles
{
    public class RoleResponseModel
    {
        public RoleResponseModel(string name, string description, IList<string> permissions)
        {
            Name = name;
            Description = description;
            Permissions = permissions;
        }

        public string Name { get; }
        public string Description { get; }
        public IList<string> Permissions { get; }
    }
}