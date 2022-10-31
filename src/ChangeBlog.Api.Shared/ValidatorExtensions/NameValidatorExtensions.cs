using ChangeBlog.Api.Shared.Validators;
using ChangeBlog.Domain.Miscellaneous;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class NameValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeName<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMinimumLengthValidator<T>((int) Name.MinLength))
            .SetValidator(new TrimmedMaximumLengthValidator<T>((int) Name.MaxLength));
}