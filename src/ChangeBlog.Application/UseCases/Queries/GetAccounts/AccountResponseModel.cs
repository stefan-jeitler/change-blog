using System;

namespace ChangeBlog.Application.UseCases.Queries.GetAccounts;

public class AccountResponseModel
{
    public AccountResponseModel(Guid id, string name, string defaultVersioningScheme,
        Guid defaultVersioningSchemeId, DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
        {
            throw new ArgumentException("Id cannot be empty.");
        }

        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
        DefaultVersioningScheme = defaultVersioningScheme ??
                                  throw new ArgumentNullException(nameof(defaultVersioningScheme));

        if (defaultVersioningSchemeId == Guid.Empty)
        {
            throw new ArgumentException("VersioningSchemeId cannot be empty.");
        }

        DefaultVersioningSchemeId = defaultVersioningSchemeId;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string DefaultVersioningScheme { get; }
    public Guid DefaultVersioningSchemeId { get; }
    public DateTimeOffset CreatedAt { get; }
}