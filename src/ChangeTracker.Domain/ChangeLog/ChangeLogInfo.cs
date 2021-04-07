using System;

namespace ChangeTracker.Domain.ChangeLog
{
    public record ChangeLogInfo
    {
        public ChangeLogInfo(Guid projectId, Guid? versionId, uint count, uint lastPosition)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;

            if (versionId.HasValue && versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
            Count = count;
            LastPosition = lastPosition;
        }

        public Guid ProjectId { get; }
        public Guid? VersionId { get; }
        public uint Count { get; }
        public uint LastPosition { get; }
    }
}