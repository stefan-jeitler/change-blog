using System;
using FluentValidation;
using FluentValidation.Validators;

namespace ChangeBlog.Api.Shared.Validators;

public sealed class TrimmedMinimumLengthValidator<T> : MinimumLengthValidator<T>
{
    public TrimmedMinimumLengthValidator(int min) : base(min)
    {
    }

    public TrimmedMinimumLengthValidator(Func<T, int> min) : base(min)
    {
    }

    public override bool IsValid(ValidationContext<T> context, string value)
        => base.IsValid(context, value?.Trim());
}