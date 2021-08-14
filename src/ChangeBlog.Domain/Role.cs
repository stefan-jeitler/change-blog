using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ChangeBlog.Domain.Authorization;
using ChangeBlog.Domain.Common;

namespace ChangeBlog.Domain
{
    public class Role
    {
        public const string DefaultUser = nameof(DefaultUser);
        public const string Support = nameof(Support);
        public const string ScrumMaster = nameof(ScrumMaster);
        public const string Developer = nameof(Developer);
        public const string ProductOwner = nameof(ProductOwner);
        public const string ProductManager = nameof(ProductManager);
        public const string PlatformManager = nameof(PlatformManager);

        public Role(Guid id, Name name, Text description, DateTime createdAt, IEnumerable<Permission> permissions)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Permissions = permissions?.ToImmutableHashSet() ?? throw new ArgumentNullException(nameof(permissions));

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid createdAt.");

            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public Name Name { get; }
        public Text Description { get; }
        public IImmutableSet<Permission> Permissions { get; }
        public DateTime CreatedAt { get; }
    }
}
