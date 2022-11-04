using System;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;

namespace ChangeBlog.Domain;

public class Product
{
    public Product(Guid accountId, Name name, VersioningScheme versioningScheme, Guid createdByUser,
        Name languageCode, DateTime createdAt)
        : this(Guid.NewGuid(), accountId, name, versioningScheme, languageCode, createdByUser, createdAt, null)
    {
    }

    public Product(Guid id, Guid accountId, Name name,
        Guid vsId, Name vsName, Text vsRegexPattern, Text vsDescription,
        Guid? vsAccountId, Guid vsCreatedByUser, DateTime? vsDeletedAt, DateTime vsCreatedAt,
        Name languageCode, Guid createdByUser,
        DateTime createdAt, DateTime? freezedAt)
        : this(id, accountId, name,
            new VersioningScheme(vsId, vsName, vsRegexPattern, vsDescription, vsAccountId, vsCreatedByUser,
                vsDeletedAt, vsCreatedAt), languageCode, createdByUser, createdAt, freezedAt)
    {
    }

    public Product(Guid id,
        Guid accountId,
        Name name,
        VersioningScheme versioningScheme,
        Name languageCode,
        Guid createdByUser,
        DateTime createdAt,
        DateTime? freezedAt)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty.");

        Id = id;

        if (accountId == Guid.Empty) throw new ArgumentException("AccountId cannot be empty.");

        AccountId = accountId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        VersioningScheme = versioningScheme ?? throw new ArgumentNullException(nameof(versioningScheme));

        LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));

        if (createdByUser == Guid.Empty) throw new ArgumentException("UserId cannot be empty.");

        CreatedByUser = createdByUser;

        if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
            throw new ArgumentException("Invalid creation date.");

        CreatedAt = createdAt;

        if (freezedAt.HasValue &&
            (freezedAt == DateTime.MinValue || freezedAt == DateTime.MaxValue))
            throw new ArgumentException("Invalid creation date.");

        FreezedAt = freezedAt;
    }

    public Guid Id { get; }
    public Guid AccountId { get; }
    public Name Name { get; }
    public VersioningScheme VersioningScheme { get; }
    public Name LanguageCode { get; }
    public Guid CreatedByUser { get; }
    public DateTime CreatedAt { get; }
    public DateTime? FreezedAt { get; }

    public bool IsFreezed => FreezedAt.HasValue;

    public Product Freeze() =>
        new(Id, AccountId, Name, VersioningScheme, LanguageCode, CreatedByUser, CreatedAt,
            DateTime.UtcNow);
}