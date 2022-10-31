using System;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Management.Api.DTOs.V1.ApiKey;

public class CreateOrUpdateApiKeyDto
{
    public string Name { get; set; }

    public DateTime? ExpiresAt { get; set; }
}

public class CreateOrUpdateApiKeyDtoValidator : AbstractValidator<CreateOrUpdateApiKeyDto>
{
    public CreateOrUpdateApiKeyDtoValidator()
    {
        RuleFor(x => x.Name).OptionalName();
        RuleFor(x => x.ExpiresAt)
            .NotEmpty()
            .WithName(ChangeBlogStrings.ExpirationDate);
    }
}