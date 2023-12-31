using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;
using JetBrains.Annotations;

namespace ChangeBlog.Management.Api.DTOs.V1.Account;

public class UpdateAccountDto
{
    public string Name { get; set; }
}

[UsedImplicitly]
public class UpdateAccountDtoValidator : AbstractValidator<UpdateAccountDto>
{
    public UpdateAccountDtoValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .MustBeName();
    }
}