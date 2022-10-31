using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared.Validators;
using ChangeBlog.Domain.ChangeLog;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class IssueValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustBeIssue<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMaximumLengthValidator<T>(Issue.MaxLength))
            .Must(x => !x.Contains(' '))
            .WithMessage(ChangeBlogStrings.WhitespacesNotAllowed)
            .WithName(nameof(Issue));
}