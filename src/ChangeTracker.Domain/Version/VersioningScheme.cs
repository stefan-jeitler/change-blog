using System;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain.Version
{
    public class VersioningScheme
    {
        public VersioningScheme(Guid id, Name name, Text regexPattern, Text description, Guid? accountId, Guid createdByUser,
            DateTime? deletedAt, DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            Id = id;

            Name = name ?? throw new ArgumentNullException(nameof(name));
            RegexPattern = regexPattern ?? throw new ArgumentNullException(nameof(name));

            if (accountId.HasValue && accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;

            if (createdByUser == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            CreatedByUser = createdByUser;

            Description = description ?? throw new ArgumentNullException(nameof(description));

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");

            CreatedAt = createdAt;

            if (deletedAt.HasValue &&
                (deletedAt.Value == DateTime.MinValue || deletedAt.Value == DateTime.MaxValue))
                throw new ArgumentException("Invalid deletion date");

            DeletedAt = deletedAt;
        }

        public Guid Id { get; }
        public Name Name { get; }
        public Text RegexPattern { get; }
        public Guid? AccountId { get; }
        public Guid CreatedByUser { get; }
        public Text Description { get; }
        public DateTime CreatedAt { get; }
        public DateTime? DeletedAt { get; }
    }
}