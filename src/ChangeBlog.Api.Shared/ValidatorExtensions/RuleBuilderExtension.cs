using System;
using ChangeBlog.Api.Shared.Validators;
using FluentValidation;

namespace ChangeBlog.Api.Shared.ValidatorExtensions;

public static class RuleBuilderExtension
{
    public static IRuleBuilderOptions<T, Guid?> NotEmptyGuid<T>(this IRuleBuilder<T, Guid?> ruleBuilder) =>
        ruleBuilder
            .SetValidator(new NullableGuidValidator<T>())
            .When((x, y) => GetValue(x, y).HasValue);

    private static Guid? GetValue<T>(T type, ValidationContext<T> ctx)
    {
        var property = typeof(T).GetProperty(ctx.PropertyName);
        return (Guid?) property?.GetValue(type);
    }
}