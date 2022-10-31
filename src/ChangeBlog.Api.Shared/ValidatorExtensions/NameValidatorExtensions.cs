using ChangeBlog.Api.Shared.Validators;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class NameValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> Name<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .NotEmpty()
            .SetValidator(new TrimmedMinimumLengthValidator<T>((int) Domain.Miscellaneous.Name.MinLength))
            .SetValidator(new TrimmedMaximumLengthValidator<T>((int) Domain.Miscellaneous.Name.MaxLength));

    public static IRuleBuilderOptions<T, string> OptionalName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .SetValidator(new TrimmedMinimumLengthValidator<T>((int) Domain.Miscellaneous.Name.MinLength))
            .SetValidator(new TrimmedMaximumLengthValidator<T>((int) Domain.Miscellaneous.Name.MaxLength))
            .When((v, ctx) => !string.IsNullOrWhiteSpace(GetValue(ctx, v)));
    }

    private static string GetValue<T>(ValidationContext<T> ctx, T model)
    {
        var property = typeof(T).GetProperty(ctx.PropertyName);
        return (string) property?.GetValue(model);
    }
}