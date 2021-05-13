using System;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;

namespace ChangeTracker.Domain
{
    public class Project
    {
        public Project(Guid accountId, Name name, VersioningScheme versioningScheme, DateTime createdAt)
            : this(Guid.NewGuid(), accountId, name, versioningScheme, createdAt, null)
        {
        }

        public Project(Guid id, Guid accountId, Name name, VersioningScheme versioningScheme, DateTime createdAt,
            DateTime? closedAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            Id = id;

            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            VersioningScheme = versioningScheme ?? throw new ArgumentNullException(nameof(versioningScheme));

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");

            CreatedAt = createdAt;

            if (closedAt.HasValue &&
                (closedAt == DateTime.MinValue || closedAt == DateTime.MaxValue))
                throw new ArgumentException("Invalid creation date.");

            ClosedAt = closedAt;
        }

        public Guid Id { get; }
        public Guid AccountId { get; }
        public Name Name { get; }
        public VersioningScheme VersioningScheme { get; }
        public DateTime CreatedAt { get; }
        public DateTime? ClosedAt { get; }

        public bool IsClosed => ClosedAt.HasValue;
    }
}