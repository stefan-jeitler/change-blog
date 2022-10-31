using ChangeBlog.Api.Shared.Validators;
using ChangeBlog.Domain.Miscellaneous;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class TextValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeText<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMaximumLengthValidator<T>((int) Text.MaxLength));
}