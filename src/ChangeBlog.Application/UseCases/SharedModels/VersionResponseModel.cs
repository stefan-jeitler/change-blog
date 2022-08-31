using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.SharedModels;

public class VersionResponseModel
{
    public VersionResponseModel(Guid versionId, string version, string name, Guid productId,
        string productName, Guid accountId,
        List<ChangeLogLineResponseModel> changeLogs, DateTimeOffset createdAt, DateTimeOffset? releasedAt,
        DateTimeOffset? deletedAt)
    {
        VersionId = Guard.Against.NullOrEmpty(versionId, nameof(versionId));
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        ProductId = Guard.Against.NullOrEmpty(productId, nameof(productId));
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        AccountId = Guard.Against.NullOrEmpty(accountId, nameof(accountId));

        if (createdAt == DateTimeOffset.MinValue || createdAt == DateTimeOffset.MaxValue)
        {
            throw new ArgumentException("Invalid creation date.");
        }

        ChangeLogs = changeLogs ?? throw new ArgumentNullException(nameof(changeLogs));
        CreatedAt = createdAt;
        ReleasedAt = releasedAt;
        DeletedAt = deletedAt;
    }

    public Guid VersionId { get; }
    public string Version { get; }
    public string Name { get; }
    public Guid ProductId { get; }
    public string ProductName { get; }
    public Guid AccountId { get; }
    public List<ChangeLogLineResponseModel> ChangeLogs { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ReleasedAt { get; }
    public DateTimeOffset? DeletedAt { get; }
}