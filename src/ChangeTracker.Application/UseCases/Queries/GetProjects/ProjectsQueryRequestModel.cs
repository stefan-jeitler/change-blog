using System;

namespace ChangeTracker.Application.UseCases.Queries.GetProjects
{
    public class ProjectsQueryRequestModel
    {
        public const ushort MaxChunkCount = 100;

        public ProjectsQueryRequestModel(Guid userId, Guid accountId, Guid? lastProjectId, ushort count)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;
            AccountId = accountId;
            LastProjectId = lastProjectId;
            Count = Math.Min(count, MaxChunkCount);
        }

        public Guid UserId { get; }
        public Guid AccountId { get; }
        public Guid? LastProjectId { get; }
        public ushort Count { get; }
    }
}