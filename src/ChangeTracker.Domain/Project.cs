using System;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;

namespace ChangeTracker.Domain
{
    public class Project
    {
        public Project(Guid accountId, Name name, VersioningScheme versioningScheme, Guid createdByUser,
            DateTime createdAt)
            : this(Guid.NewGuid(), accountId, name, versioningScheme, createdByUser, createdAt, null)
        {
        }

        public Project(Guid id, Guid accountId, Name name,
            Guid versioningSchemeId, Name versioningSchemeName, Text regexPattern, Text description,
            DateTime versioningSchemeCreatedAt, DateTime? versioningSchemeDeletedAt, Guid createdByUser,
            DateTime createdAt, DateTime? closedAt)
            : this(id, accountId, name,
                new VersioningScheme(versioningSchemeId, versioningSchemeName, regexPattern, accountId, description,
                    versioningSchemeCreatedAt, versioningSchemeDeletedAt), createdByUser, createdAt, closedAt)
        {
        }

        public Project(Guid id, Guid accountId, Name name, VersioningScheme versioningScheme, Guid createdByUser,
            DateTime createdAt,
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

            if (createdByUser == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            CreatedByUser = createdByUser;

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
        public Guid CreatedByUser { get; }
        public DateTime CreatedAt { get; }
        public DateTime? ClosedAt { get; }

        public bool IsClosed => ClosedAt.HasValue;
        public Project Close() => new(Id, AccountId, Name, VersioningScheme, CreatedByUser, CreatedAt, DateTime.UtcNow);
    }
}