using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

// ReSharper disable InvertIf

namespace ChangeBlog.Domain.ChangeLog
{
    public class ChangeLogLine
    {
        public const int MaxIssues = 10;
        public const int MaxLabels = 5;

        public ChangeLogLine(Guid id, Guid? versionId, Guid productId, ChangeLogText text, uint position,
            Guid createdByUser, DateTime createdAt, DateTime? deletedAt = null)
            : this(id, versionId, productId, text, position, createdAt, Enumerable.Empty<Label>(),
                Enumerable.Empty<Issue>(), createdByUser, deletedAt)
        {
        }

        public ChangeLogLine(Guid id, Guid? versionId, Guid productId, ChangeLogText text, int position,
            DateTime createdAt, IEnumerable<Label> labels, IEnumerable<Issue> issues, Guid createdByUser,
            DateTime? deletedAt = null)
            : this(id, versionId, productId, text, (uint) position, createdAt, labels, issues, createdByUser, deletedAt)
        {
        }

        public ChangeLogLine(Guid id, Guid? versionId, Guid productId, ChangeLogText text, uint position,
            DateTime createdAt, IEnumerable<Label> labels, IEnumerable<Issue> issues, Guid createdByUser,
            DateTime? deletedAt = null)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id must not be empty.", nameof(id));

            Id = id;

            if (versionId.HasValue && versionId.Value == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;

            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId must not be empty.", nameof(productId));

            ProductId = productId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Position = position;
            Labels = Populate(labels ?? throw new ArgumentNullException(nameof(labels)), MaxLabels);
            Issues = Populate(issues ?? throw new ArgumentNullException(nameof(issues)), MaxIssues);

            if (createdByUser == Guid.Empty)
                throw new ArgumentException("CreatedByUser cannot be empty.");

            CreatedByUser = createdByUser;

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.", nameof(createdAt));

            CreatedAt = createdAt;

            if (deletedAt.HasValue && (deletedAt.Value == DateTime.MinValue || deletedAt.Value == DateTime.MaxValue))
                throw new ArgumentException("Invalid deletion date.");

            DeletedAt = deletedAt;
        }

        public ChangeLogLine(Guid? versionId, Guid productId, ChangeLogText text, uint position, Guid createdByUser)
            : this(Guid.NewGuid(), versionId, productId, text, position, createdByUser, DateTime.UtcNow)
        {
        }

        public Guid Id { get; }
        public Guid? VersionId { get; }
        public Guid ProductId { get; }
        public ChangeLogText Text { get; }
        public uint Position { get; }
        public IImmutableSet<Label> Labels { get; private set; }
        public IImmutableSet<Issue> Issues { get; private set; }
        public Guid CreatedByUser { get; }
        public DateTime CreatedAt { get; }
        public DateTime? DeletedAt { get; }

        public bool IsPending => !VersionId.HasValue;

        public int AvailableLabelPlaces => MaxLabels - Labels.Count;

        public int AvailableIssuePlaces => MaxIssues - Issues.Count;

        public void AddLabel(Label label)
        {
            AddLabels(new List<Label>(1) {label});
        }

        public void AddLabels(IReadOnlyCollection<Label> labels)
        {
            if (Labels.Count + labels.Count >= MaxLabels)
                throw new ArgumentException($"Too many labels. Max. {MaxLabels} labels.");

            Labels = Labels.Union(labels);
        }

        public void RemoveLabel(Label label)
        {
            RemoveLabels(new List<Label>(1) {label});
        }

        public void RemoveLabels(IEnumerable<Label> labels)
        {
            Labels = Labels.Except(labels);
        }

        public void AddIssue(Issue issue)
        {
            AddIssues(new List<Issue>(1) {issue});
        }

        public void AddIssues(IReadOnlyCollection<Issue> issues)
        {
            if (Issues.Count + issues.Count > MaxIssues)
                throw new ArgumentException("");

            Issues = Issues.Union(issues);
        }

        public void RemoveIssue(Issue issue)
        {
            RemoveIssues(new List<Issue>(1) {issue});
        }

        public void RemoveIssues(IEnumerable<Issue> issues)
        {
            Issues = Issues.Except(issues);
        }

        public ChangeLogLine AssignToVersion(Guid versionId, uint position)
        {
            if (!IsPending)
                throw new ArgumentException("Not pending lines can't be assigned.");

            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            return new ChangeLogLine(Id, versionId, ProductId, Text, position, CreatedAt, Labels, Issues, CreatedByUser,
                DeletedAt);
        }

        private static ImmutableHashSet<T> Populate<T>(IEnumerable<T> items, ushort maxCount)
        {
            var itemsSet = items.ToImmutableHashSet();

            if (itemsSet.Count > maxCount)
            {
                var name = $"{typeof(T).Name}s";
                throw new ArgumentException($"Too many {name}. Max. {maxCount} {name}.");
            }

            return itemsSet;
        }
    }
}