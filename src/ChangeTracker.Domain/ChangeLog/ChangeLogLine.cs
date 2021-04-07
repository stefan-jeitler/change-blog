using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace ChangeTracker.Domain.ChangeLog
{
    public record ChangeLogLine
    {
        public const int MaxLabels = 5;

        public ChangeLogLine(Guid id, Guid? versionId, Guid projectId, ChangeLogText text, uint position,
            DateTime createdAt, IEnumerable<Label> labels = null, IEnumerable<Issue> issues = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must not be empty", nameof(id));

            Id = id;

            if (versionId.HasValue && versionId.Value == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;

            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId must not be empty.", nameof(projectId));

            ProjectId = projectId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Position = position;

            Labels = labels?
                .Take(MaxLabels)
                .ToImmutableHashSet() ?? ImmutableHashSet<Label>.Empty;

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date", nameof(createdAt));

            CreatedAt = createdAt;
        }

        public ChangeLogLine(Guid? versionId, Guid projectId, ChangeLogText text, uint position)
            : this(Guid.NewGuid(), versionId, projectId, text, position, DateTime.UtcNow)
        {
        }

        public Guid Id { get; }
        public Guid? VersionId { get; }
        public Guid ProjectId { get; }
        public ChangeLogText Text { get; }
        public uint Position { get; }
        public IImmutableSet<Label> Labels { get; private set; }
        public DateTime CreatedAt { get; }

        public bool IsPending => !VersionId.HasValue;

        public bool TryAddLabel(Label label)
        {
            if (Labels.Count >= MaxLabels)
                return false;

            if (!IsPending)
                return false;

            var count = Labels.Count;
            Labels = Labels.Add(label);

            return count != Labels.Count;
        }

        public bool TryRemoveLabel(Label label)
        {
            if (!IsPending)
                return false;

            var count = Labels.Count;
            Labels = Labels.Remove(label);

            return count != Labels.Count;
        }
    }
}