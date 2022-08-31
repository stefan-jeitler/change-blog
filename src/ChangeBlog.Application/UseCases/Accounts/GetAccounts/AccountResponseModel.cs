using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.Accounts.GetAccounts;

public class AccountResponseModel
{
    public AccountResponseModel(Guid id, string name, string defaultVersioningScheme,
        Guid defaultVersioningSchemeId, DateTimeOffset createdAt)
    {
        Id = Guard.Against.NullOrEmpty(id, nameof(id));
        Name = Guard.Against.Null(name, nameof(name));

        DefaultVersioningScheme = defaultVersioningScheme ??
                                  throw new ArgumentNullException(nameof(defaultVersioningScheme));

        DefaultVersioningSchemeId = Guard.Against.NullOrEmpty(defaultVersioningSchemeId, nameof(defaultVersioningSchemeId));
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string DefaultVersioningScheme { get; }
    public Guid DefaultVersioningSchemeId { get; }
    public DateTimeOffset CreatedAt { get; }
}