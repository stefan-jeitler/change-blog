using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain
{
    public class Role
    {
        public Role(Guid id, Name name, Text description, DateTime createdAt, IEnumerable<Name> permissions)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty.");

            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Permissions = permissions?.ToImmutableList() ?? throw new ArgumentNullException(nameof(permissions));

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid createdAt.");

            CreatedAt = createdAt;
        }

        public Role(Guid id, Name name, Text description, Name permission, DateTime createdAt)
            : this(id, name, description, createdAt, new List<Name>(0) {permission})
        {
        }


        public Guid Id { get; }
        public Name Name { get; }
        public Text Description { get; }
        public IImmutableList<Name> Permissions { get; set; }
        public DateTime CreatedAt { get; }
    }
}