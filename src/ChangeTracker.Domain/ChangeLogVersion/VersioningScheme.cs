﻿using System;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain.ChangeLogVersion
{
    public record VersioningScheme
    {
        public VersioningScheme(Guid id, Name name, Text regexPattern, Guid? accountId, Text description,
            DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            Id = id;

            Name = name ?? throw new ArgumentNullException(nameof(name));
            RegexPattern = regexPattern ?? throw new ArgumentNullException(nameof(name));

            if (accountId.HasValue && accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;
            Description = description ?? throw new ArgumentNullException(nameof(description));

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");

            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public Name Name { get; }
        public Text RegexPattern { get; }
        public Guid? AccountId { get; }
        public Text Description { get; }
        public DateTime CreatedAt { get; }
    }
}