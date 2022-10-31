using ChangeBlog.Api.Shared.Validators;
using ChangeBlog.Domain.ChangeLog;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class ChangeLogLineValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeChangeLogText<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMinimumLengthValidator<T>(ChangeLogText.MinLength))
            .SetValidator(new TrimmedMaximumLengthValidator<T>(ChangeLogText.MaxLength));
}