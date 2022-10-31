using System.Collections.Generic;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Api.DTOs.V1.Version;

public class AddOrUpdateVersionDto
{
    public string Name { get; set; }

    public List<AddOrUpdateChangeLogLineDto> ChangeLogLines { get; set; }

    public bool ReleaseImmediately { get; set; }
}

public class AddOrUpdateVersionDtoValidator : AbstractValidator<AddOrUpdateVersionDto>
{
    public AddOrUpdateVersionDtoValidator()
    {
        RuleFor(x => x.Name).MustBeOptionalName();
        RuleForEach(x => x.ChangeLogLines).SetValidator(new AddOrUpdateChangeLogLineDtoValidator());
    }
}