using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;
using JetBrains.Annotations;
using D = ChangeBlog.Domain.Miscellaneous;

namespace ChangeBlog.Management.Api.DTOs.V1.Account;

public class CreateAccountDto
{
    public string Name { get; set; }
}

[UsedImplicitly]
public class CreateAccountDtoValidator : AbstractValidator<CreateAccountDto>
{
    public CreateAccountDtoValidator()
    {
        RuleFor(x => x.Name).MustBeName();
    }
}