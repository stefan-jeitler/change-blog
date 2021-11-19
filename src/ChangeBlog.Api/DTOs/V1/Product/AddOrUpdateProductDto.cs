using System;
using System.ComponentModel.DataAnnotations;
using ChangeBlog.Api.Authorization.RequestBodyIdentifiers;

namespace ChangeBlog.Api.DTOs.V1.Product;

public class AddOrUpdateProductDto : IContainsAccountId
{
    [Required] public string Name { get; set; }
    public Guid? VersioningSchemeId { get; set; }
    [Required] public string LanguageCode { get; set; }
    [Required] public Guid AccountId { get; set; }
}