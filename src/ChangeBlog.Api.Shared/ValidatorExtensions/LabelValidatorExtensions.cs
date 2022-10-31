using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.Validators;
using ChangeBlog.Domain.ChangeLog;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class LabelValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeLabel<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMinimumLengthValidator<T>(Label.MinLength))
            .SetValidator(new TrimmedMaximumLengthValidator<T>(Label.MaxLength))
            .Must(x => !x.Contains(' '))
            .WithMessage(ChangeBlogStrings.WhitespacesNotAllowed)
            .Matches(Label.OnlyAlphanumericCharsRegex)
            .WithMessage(ChangeBlogStrings.OnlyAlphanumericCharsAllowed)
            .WithName(nameof(Label));
}