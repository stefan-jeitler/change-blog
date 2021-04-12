using System;

namespace ChangeTracker.Domain.ChangeLog
{
    public record ChangeLogsMetadata
    {
        public const int MaxChangeLogLines = 100;

        public ChangeLogsMetadata(Guid projectId, Guid? versionId, uint count, int lastPosition)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;

            if (versionId.HasValue && versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
            Count = count;

            if (lastPosition < -1)
                throw new ArgumentException("LastPosition must not be smaller than -1.");

            LastPosition = count == 0 ? -1 : lastPosition;
        }

        public Guid ProjectId { get; }
        public Guid? VersionId { get; }
        public uint Count { get; }
        public int LastPosition { get; }

        public uint RemainingPositionsToAdd => MaxChangeLogLines - Count;
        public uint NextFreePosition => (uint) (LastPosition + 1);
        public bool IsPositionAvailable => RemainingPositionsToAdd > 0;
    }
}