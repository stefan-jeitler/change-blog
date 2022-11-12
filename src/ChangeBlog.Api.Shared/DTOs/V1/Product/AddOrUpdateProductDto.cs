using System;
using ChangeBlog.Api.Shared.Authorization.RequestBodyIdentifiers;
using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Api.Shared.DTOs.V1.Product;

public class AddOrUpdateProductDto : IContainsAccountId
{
    public string Name { get; set; }
    public Guid? VersioningSchemeId { get; set; }
    public string LanguageCode { get; set; }
    public Guid AccountId { get; set; }
}

public class AddOrUpdateProductValidator : AbstractValidator<AddOrUpdateProductDto>
{
    public AddOrUpdateProductValidator()
    {
        RuleFor(x => x.Name).MustBeName();
        RuleFor(x => x.VersioningSchemeId).NotEmptyGuid();
        RuleFor(x => x.LanguageCode).MustBeName();
        RuleFor(x => x.AccountId).NotEmpty();
    }
}