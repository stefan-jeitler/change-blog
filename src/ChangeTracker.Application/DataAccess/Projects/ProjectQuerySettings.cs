using System;

namespace ChangeTracker.Application.DataAccess.Projects
{
    public class ProjectQuerySettings
    {
        public ProjectQuerySettings(Guid accountId, Guid userId, Guid? lastProjectId = null, ushort limit = 100,
            bool includeClosedProjects = false)
        {
            AccountId = accountId;
            UserId = userId;
            LastProjectId = lastProjectId;
            Limit = limit;
            IncludeClosedProjects = includeClosedProjects;
        }

        public Guid UserId { get; }
        public Guid AccountId { get; }
        public Guid? LastProjectId { get; }
        public ushort Limit { get; }
        public bool IncludeClosedProjects { get; }
    }
}