using System.Collections.Generic;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Api.DTOs.V1.Version;

public class AddVersionDto
{
    public string Version { get; set; }
    public string Name { get; set; }
    public List<AddOrUpdateChangeLogLineDto> ChangeLogLines { get; set; }
    public bool ReleaseImmediately { get; set; }
}

public class AddVersionDtoValidator : AbstractValidator<AddVersionDto>
{
    public AddVersionDtoValidator()
    {
        RuleFor(x => x.Version);
        RuleFor(x => x.Name).MustBeOptionalName();
        RuleForEach(x => x.ChangeLogLines)
            .SetValidator(new AddOrUpdateChangeLogLineDtoValidator());
    }
}