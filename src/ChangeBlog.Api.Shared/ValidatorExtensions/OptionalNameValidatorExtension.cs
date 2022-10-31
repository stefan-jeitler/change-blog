using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class OptionalNameValidatorExtension
{
    public static IRuleBuilderOptions<T, string> MustBeOptionalName<T>(this IRuleBuilder<T, string> ruleBuilder) =>
        ruleBuilder
            .MustBeName()
            .When((t, v) => !string.IsNullOrWhiteSpace(GetValue(t, v)));

    private static string GetValue<T>(T type, ValidationContext<T> ctx)
    {
        var property = typeof(T).GetProperty(ctx.PropertyName);
        return (string) property?.GetValue(type);
    }
}