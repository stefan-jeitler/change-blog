using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Management.Api.DTOs.V1;

public class UpdateUserProfileDto
{
    /// <summary>
    ///     Windows or Olson id
    /// </summary>
    public string Timezone { get; set; }

    public string Culture { get; set; }
}

public class UpdateUserProfileDtoValidator : AbstractValidator<UpdateUserProfileDto>
{
    public UpdateUserProfileDtoValidator()
    {
        RuleFor(x => x.Culture).OptionalName();
        RuleFor(x => x.Timezone).OptionalName();
    }
}