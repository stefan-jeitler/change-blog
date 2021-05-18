using System;

namespace ChangeTracker.Application.UseCases.Queries.GetProjects
{
    public class ProjectsQueryRequestModel
    {
        public const ushort MaxLimit = 100;

        public ProjectsQueryRequestModel(Guid userId, Guid accountId, Guid? lastProjectId, ushort limit,
            bool includeClosedProjects)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;
            LastProjectId = lastProjectId;
            Limit = Math.Min(limit, MaxLimit);
            IncludeClosedProjects = includeClosedProjects;
        }

        public Guid UserId { get; }
        public Guid AccountId { get; }
        public Guid? LastProjectId { get; }
        public ushort Limit { get; }
        public bool IncludeClosedProjects { get; }
    }
}