using System;

namespace ChangeBlog.Application.UseCases.Queries.GetProducts;

public class ProductResponseModel
{
    public ProductResponseModel(Guid id, Guid accountId, string accountName, string name,
        Guid versioningSchemeId, string versioningScheme, string languageCode, string createdByUser,
        DateTimeOffset createdAt, DateTimeOffset? closedAt)
    {
        Id = id;
        AccountId = accountId;
        AccountName = accountName ?? throw new ArgumentNullException(nameof(accountName));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        VersioningSchemeId = versioningSchemeId;
        VersioningScheme = versioningScheme ?? throw new ArgumentNullException(nameof(versioningScheme));
        LanguageCode = languageCode ?? throw new ArgumentNullException(nameof(languageCode));
        CreatedByUser = createdByUser ?? throw new ArgumentNullException(nameof(createdByUser));
        CreatedAt = createdAt;
        ClosedAt = closedAt;
    }

    public Guid Id { get; }
    public Guid AccountId { get; }
    public string AccountName { get; }
    public string Name { get; }
    public Guid VersioningSchemeId { get; }
    public string VersioningScheme { get; }
    public string LanguageCode { get; }
    public string CreatedByUser { get; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ClosedAt { get; }
}