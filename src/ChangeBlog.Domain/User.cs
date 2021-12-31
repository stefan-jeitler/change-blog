using System;
using ChangeBlog.Domain.Miscellaneous;

namespace ChangeBlog.Domain;

public class User
{
    public User(Guid id, Email email, Name firstName, Name lastName, Name timeZone, DateTime? deletedAt,
        DateTime createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty");
        }

        Id = id;
        Email = email ?? throw new ArgumentNullException(nameof(email));
        FirstName = firstName ?? throw new ArgumentNullException(nameof(firstName));
        LastName = lastName ?? throw new ArgumentNullException(nameof(lastName));
        TimeZone = timeZone ?? throw new ArgumentNullException(nameof(timeZone));

        if (deletedAt.HasValue &&
            (deletedAt == DateTime.MinValue || deletedAt == DateTime.MaxValue))
        {
            throw new ArgumentException("Invalid creation date.");
        }

        DeletedAt = deletedAt;

        if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
        {
            throw new ArgumentException("Invalid creation date.");
        }

        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public Email Email { get; }
    public Name FirstName { get; }
    public Name LastName { get; }

    /// <summary>
    ///     OlsonId
    /// </summary>
    public Name TimeZone { get; }

    public DateTime? DeletedAt { get; }

    public DateTime CreatedAt { get; }
}