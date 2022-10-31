using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;
using JetBrains.Annotations;
using D = ChangeBlog.Domain.Miscellaneous;

namespace ChangeBlog.Management.Api.DTOs.V1.Account;

public class CreateOrUpdateAccountDto
{
    public string Name { get; set; }
}

[UsedImplicitly]
public class CreateOrUpdateAccountDtoValidator : AbstractValidator<CreateOrUpdateAccountDto>
{
    public CreateOrUpdateAccountDtoValidator()
    {
        RuleFor(x => x.Name).Name();
    }
}