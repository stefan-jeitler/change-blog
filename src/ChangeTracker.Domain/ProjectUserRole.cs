using System;

namespace ChangeTracker.Domain
{
    public record ProjectUserRole
    {
        public ProjectUserRole(Guid userId, Guid projectId, Guid roleId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            UserId = userId;

            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;

            if (roleId == Guid.Empty)
                throw new ArgumentException("RoleId cannot be empty.");

            RoleId = roleId;
        }

        public Guid UserId { get; }
        public Guid ProjectId { get; }
        public Guid RoleId { get; }
    }
}