using System;
using FluentValidation;
using FluentValidation.Validators;

namespace ChangeBlog.Api.Shared.Validators;

public class NullableGuidValidator<T> : NotEmptyValidator<T, Guid?>
{
    public override bool IsValid(ValidationContext<T> context, Guid? value) =>
        base.IsValid(context, value.HasValue && value.Value == Guid.Empty ? null : value);
}