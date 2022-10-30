using System;
using ChangeBlog.Management.Api.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Management.Api.DTOs.V1.ApiKey;

public class GenerateApiKeyDto
{
    public string Title { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class CreateOrUpdateApiKeyDtoValidator : AbstractValidator<GenerateApiKeyDto>
{
    public CreateOrUpdateApiKeyDtoValidator()
    {
        RuleFor(x => x.Title).OptionalName();
        RuleFor(x => x.ExpiresAt).NotEmpty();
    }
}