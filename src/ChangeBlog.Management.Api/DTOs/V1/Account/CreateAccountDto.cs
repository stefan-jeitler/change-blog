using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;
using JetBrains.Annotations;

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
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .MustBeName();
    }
}