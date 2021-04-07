using System;
using ChangeTracker.Domain.ChangeLogVersion;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain
{
    public record Project
    {
        public Project(Guid accountId, Name name, VersioningScheme versioningScheme, DateTime createdAt)
            : this(Guid.NewGuid(), accountId, name, versioningScheme, createdAt, null)
        {
        }

        public Project(Guid id, Guid accountId, Name name, VersioningScheme versioningScheme, DateTime createdAt,
            DateTime? deletedAt)
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

            if (deletedAt.HasValue &&
                (deletedAt == DateTime.MinValue || deletedAt == DateTime.MaxValue))
                throw new ArgumentException("Invalid creation date.");

            DeletedAt = deletedAt;
        }

        public Guid Id { get; }
        public Guid AccountId { get; }
        public Name Name { get; }
        public VersioningScheme VersioningScheme { get; }
        public DateTime CreatedAt { get; }
        public DateTime? DeletedAt { get; }
    }
}