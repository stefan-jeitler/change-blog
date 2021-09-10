using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;

namespace ChangeBlog.Domain.ChangeLog
{
    public class ChangeLogs
    {
        public const int MaxLines = 100;

        public ChangeLogs(IReadOnlyCollection<ChangeLogLine> lines)
        {
            VerifyNoDuplicatesExist(lines);
            Lines = lines?.ToImmutableList() ?? throw new ArgumentNullException(nameof(lines));
            VersionId = GetVersionId(lines);
        }

        public Guid? VersionId { get; }
        public IImmutableList<ChangeLogLine> Lines { get; }
        public int Count => Lines.Count;

        public int LastPosition => Lines
            .Select(x => (int) x.Position)
            .DefaultIfEmpty(-1)
            .Max();

        public uint RemainingPositionsToAdd => MaxLines - (uint) Count;
        public uint NextFreePosition => (uint) (LastPosition + 1);
        public bool IsPositionAvailable => RemainingPositionsToAdd > 0;

        private static void VerifyNoDuplicatesExist(IEnumerable<ChangeLogLine> lines)
        {
            var duplicates = lines
                .GroupBy(x => x.Text.Value.ToLower())
                .Where(x => x.Skip(1).Any())
                .Select(x => x);

            if (duplicates.Any())
                throw new ArgumentException("Lines with same text are not allowed.");
        }

        public bool ContainsText(ChangeLogText other)
        {
            return Lines
                .Any(x => x.Text.Equals(other));
        }

        public IEnumerable<ChangeLogLine> FindDuplicateTexts(IEnumerable<ChangeLogLine> others)
        {
            var texts = others
                .Select(x => x.Text.Value.ToLower(CultureInfo.InvariantCulture))
                .ToHashSet();

            return Lines.Where(x => texts.Contains(x.Text.Value.ToLower(CultureInfo.InvariantCulture)));
        }

        private static Guid? GetVersionId(IReadOnlyCollection<ChangeLogLine> lines)
        {
            if (lines.Any(x => x.VersionId.HasValue && x.VersionId.Value == Guid.Empty))
                throw new ArgumentException("VersionId must not be empty.");

            var versionId = lines
                .Select(x => x.VersionId ?? Guid.Empty)
                .DefaultIfEmpty(Guid.Empty)
                .Distinct()
                .Single();

            return versionId == Guid.Empty
                ? null
                : versionId;
        }
    }
}