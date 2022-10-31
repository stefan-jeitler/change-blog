using System;
using FluentValidation;
using FluentValidation.Validators;

namespace ChangeBlog.Api.Shared.Validators;

public sealed class TrimmedMaximumLengthValidator<T> : MaximumLengthValidator<T>
{
    public TrimmedMaximumLengthValidator(int max) : base(max)
    {
    }

    public TrimmedMaximumLengthValidator(Func<T, int> max) : base(max)
    {
    }

    public override bool IsValid(ValidationContext<T> context, string value)
        => base.IsValid(context, value?.Trim());
}