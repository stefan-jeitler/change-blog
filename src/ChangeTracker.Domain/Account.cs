using System;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain
{
    public record Account
    {
        public Account(Guid id, Name name, Guid? defaultVersioningSchemeId, DateTime createdAt, DateTime? deletedAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DefaultVersioningSchemeId = defaultVersioningSchemeId;

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");

            CreatedAt = createdAt;

            if (deletedAt.HasValue &&
                (deletedAt.Value == DateTime.MinValue || deletedAt.Value == DateTime.MaxValue))
                throw new ArgumentException("Invalid deletion date.");

            DeletedAt = deletedAt;
        }

        public Guid Id { get; }
        public Name Name { get; }
        public Guid? DefaultVersioningSchemeId { get; }
        public DateTime CreatedAt { get; }
        public DateTime? DeletedAt { get; }
    }
}