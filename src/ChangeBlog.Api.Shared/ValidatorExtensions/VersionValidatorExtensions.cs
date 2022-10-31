using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.Validators;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Version;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class VersionValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeVersion<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMaximumLengthValidator<T>(ClVersionValue.MaxLength))
            .Must(x => !x.Contains(' '))
            .WithMessage(ChangeBlogStrings.WhitespacesNotAllowed)
            .WithName(nameof(Issue));
}