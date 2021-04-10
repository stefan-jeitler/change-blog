using System;

namespace ChangeTracker.Domain.ChangeLog
{
    public record ChangeLogInfo
    {
        public const int MaxChangeLogLines = 100;

        public ChangeLogInfo(Guid projectId, Guid? versionId, uint count, int lastPosition)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;

            if (versionId.HasValue && versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
            Count = count;
            LastPosition = count == 0 ? -1 : lastPosition;
        }

        public Guid ProjectId { get; }
        public Guid? VersionId { get; }
        public uint Count { get; }
        public int LastPosition { get; }

        public uint RemainingPositionsToAdd => MaxChangeLogLines - Count;
        public int NextFreePosition => LastPosition + 1;
        public bool IsPositionAvailable => RemainingPositionsToAdd > 0;
    }
}