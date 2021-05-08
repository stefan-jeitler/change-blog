using System;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Domain
{
    public class User
    {
        public User(Guid id, Email email, Name firstName, Name lastName, DateTime? deletedAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id cannot be empty");

            Id = id;
            Email = email ?? throw new ArgumentNullException(nameof(email));
            FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
            LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));

            if (deletedAt.HasValue &&
                (deletedAt == DateTime.MinValue || deletedAt == DateTime.MaxValue))
            {
                throw new ArgumentException("Invalid creation date.");
            }

            DeletedAt = deletedAt;
        }

        public Guid Id { get; }
        public Email Email { get; }
        public Name FirstName { get; }
        public Name LastName { get; }
        public DateTime? DeletedAt { get; }
    }
}