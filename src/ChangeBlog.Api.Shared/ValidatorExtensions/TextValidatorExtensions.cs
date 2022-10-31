using ChangeBlog.Api.Shared.Validators;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class TextValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> Text<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMaximumLengthValidator<T>((int) Domain.Miscellaneous.Text.MaxLength));
}