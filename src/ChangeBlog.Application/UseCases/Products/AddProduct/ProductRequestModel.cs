using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.Products.AddProduct;

public class ProductRequestModel
{
    public ProductRequestModel(Guid accountId, string name, Guid? versioningSchemeId, string languageCode,
        Guid userId)
    {
        AccountId = Guard.Against.NullOrEmpty(accountId, nameof(accountId));
        Name = name ?? throw new ArgumentNullException(nameof(name));

        if (versioningSchemeId.HasValue && versioningSchemeId.Value == Guid.Empty)
        {
            throw new ArgumentException("VersioningSchemeId cannot be empty.");
        }

        VersioningSchemeId = versioningSchemeId;
        LanguageCode = languageCode?.ToLower() ?? throw new ArgumentNullException(nameof(languageCode));
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
    }

    public Guid AccountId { get; }
    public string Name { get; }
    public Guid? VersioningSchemeId { get; }
    public string LanguageCode { get; }
    public Guid UserId { get; }
}