using System;
using ChangeBlog.Application.UseCases.Products.GetProducts;

namespace ChangeBlog.Api.DTOs.V1.Product;

public class ProductDto
{
    public ProductDto(Guid id, Guid accountId, string accountName, string name, Guid versioningSchemeId,
        string versioningScheme, string languageCode, string createdByUser, DateTimeOffset createdAt, bool isFreezed)
    {
        Id = id;
        AccountId = accountId;
        AccountName = accountName;
        Name = name;
        VersioningSchemeId = versioningSchemeId;
        VersioningScheme = versioningScheme;
        LanguageCode = languageCode;
        CreatedByUser = createdByUser;
        CreatedAt = createdAt;
        IsFreezed = isFreezed;
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
    public bool IsFreezed { get; }

    public static ProductDto FromResponseModel(ProductResponseModel m) =>
        new ProductDto(
            m.Id,
            m.AccountId,
            m.AccountName,
            m.Name,
            m.VersioningSchemeId,
            m.VersioningScheme,
            m.LanguageCode,
            m.CreatedByUser,
            m.CreatedAt,
            m.FreezedAt.HasValue);
}