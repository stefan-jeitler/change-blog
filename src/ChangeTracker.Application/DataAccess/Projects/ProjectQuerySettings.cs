using System;

namespace ChangeTracker.Application.DataAccess.Projects
{
    public class ProjectQuerySettings
    {
        public ProjectQuerySettings(Guid accountId, Guid userId, Guid? lastProjectId, ushort count,
            bool includeClosedProjects = false)
        {
            AccountId = accountId;
            UserId = userId;
            LastProjectId = lastProjectId;
            Count = count;
            IncludeClosedProjects = includeClosedProjects;
        }

        public Guid UserId { get; }
        public Guid AccountId { get; }
        public Guid? LastProjectId { get; }
        public ushort Count { get; }
        public bool IncludeClosedProjects { get; }
    }
}