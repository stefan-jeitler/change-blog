using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.Accounts.GetAccounts;

public class AccountResponseModel
{
    public AccountResponseModel(Guid id, string name, string defaultVersioningScheme,
        Guid defaultVersioningSchemeId, string createdBy, DateTimeOffset createdAt, bool wasCreatedByMyself,
        AccountStatsResponseModel accountStats)
    {
        Id = Guard.Against.NullOrEmpty(id, nameof(id));
        Name = Guard.Against.Null(name, nameof(name));

        DefaultVersioningScheme = defaultVersioningScheme ??
                                  throw new ArgumentNullException(nameof(defaultVersioningScheme));

        DefaultVersioningSchemeId =
            Guard.Against.NullOrEmpty(defaultVersioningSchemeId, nameof(defaultVersioningSchemeId));

        CreatedBy = Guard.Against.NullOrWhiteSpace(createdBy, nameof(createdBy));
        CreatedAt = createdAt;
        WasCreatedByMyself = wasCreatedByMyself;

        ArgumentNullException.ThrowIfNull(accountStats);
        AccountStats = accountStats;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string DefaultVersioningScheme { get; }
    public Guid DefaultVersioningSchemeId { get; }
    public string CreatedBy { get; }
    public DateTimeOffset CreatedAt { get; }
    public bool WasCreatedByMyself { get; }

    public AccountStatsResponseModel AccountStats { get; }
}