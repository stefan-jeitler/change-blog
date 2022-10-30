using System;
using ChangeBlog.Management.Api.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Management.Api.DTOs.V1.ApiKey;

public class UpdateApiKeyDto
{
    public string Title { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public class UpdateApiKeyDtoValidator : AbstractValidator<UpdateApiKeyDto>
{
    public UpdateApiKeyDtoValidator()
    {
        RuleFor(x => x.Title).OptionalName();
    }
}