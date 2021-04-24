using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ChangeTracker.Domain.ChangeLog
{
    public record ChangeLogsMetadata
    {
        public const int MaxChangeLogLines = 100;

        private ChangeLogsMetadata(Guid projectId, Guid? versionId, ImmutableHashSet<ChangeLogText> changeLogTexts,
            uint count,
            int lastPosition)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;

            if (versionId.HasValue && versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            Texts = changeLogTexts ?? throw new ArgumentNullException(nameof(changeLogTexts));
            VersionId = versionId;
            Count = count;

            if (lastPosition < -1)
                throw new ArgumentException("LastPosition must not be smaller than -1.");

            LastPosition = count == 0 ? -1 : lastPosition;
        }

        public Guid ProjectId { get; }
        public Guid? VersionId { get; }
        public IImmutableSet<ChangeLogText> Texts { get; }
        public uint Count { get; }
        public int LastPosition { get; }

        public uint RemainingPositionsToAdd => MaxChangeLogLines - Count;
        public uint NextFreePosition => (uint) (LastPosition + 1);
        public bool IsPositionAvailable => RemainingPositionsToAdd > 0;

        public static ChangeLogsMetadata Create(Guid projectId, IReadOnlyCollection<ChangeLogLine> lines)
        {
            VerifyProjectId(projectId, lines);
            var versionId = GetVersionId(lines);
            var lastPosition = GetLastPosition(lines);
            var changeLogTexts = GetTexts(lines);

            return new ChangeLogsMetadata(projectId,
                versionId != Guid.Empty ? versionId : null,
                changeLogTexts,
                (uint) lines.Count,
                lastPosition);
        }

        private static ImmutableHashSet<ChangeLogText> GetTexts(IReadOnlyCollection<ChangeLogLine> lines)
        {
            var containsDuplicates = lines
                .GroupBy(x => x.Text)
                .Any(x => x.Skip(1).Any());

            if (containsDuplicates)
                throw new ArgumentException("Duplicate Texts are not allowed.");

            return lines
                .Select(x => x.Text)
                .ToImmutableHashSet();
        }

        private static void VerifyProjectId(Guid projectId, IReadOnlyCollection<ChangeLogLine> lines)
        {
            var p = lines
                .Select(x => x.ProjectId)
                .DefaultIfEmpty(projectId)
                .Distinct()
                .Single();

            if (projectId != p)
                throw new ArgumentException("ProjectId mismatch.");
        }

        private static int GetLastPosition(IEnumerable<ChangeLogLine> lines)
        {
            return lines
                .Select(x => (int) x.Position)
                .DefaultIfEmpty(-1)
                .OrderBy(x => x)
                .Last();
        }

        private static Guid GetVersionId(IEnumerable<ChangeLogLine> lines)
        {
            return lines
                .Select(x => x.VersionId ?? Guid.Empty)
                .DefaultIfEmpty(Guid.Empty)
                .Distinct()
                .Single();
        }
    }
}