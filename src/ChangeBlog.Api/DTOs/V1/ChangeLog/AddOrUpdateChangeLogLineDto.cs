using System.Collections.Generic;
using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog;

public class AddOrUpdateChangeLogLineDto
{
    public string Text { get; set; }

    public List<string> Labels { get; set; }

    public List<string> Issues { get; set; }
}

public class AddOrUpdateChangeLogLineDtoValidator : AbstractValidator<AddOrUpdateChangeLogLineDto>
{
    public AddOrUpdateChangeLogLineDtoValidator()
    {
        Transform(x => x.Text, x => x?.Trim()).Text();

        RuleFor(x => x.Issues).NotEmpty();
        TransformForEach(x => x.Issues, x => x?.Trim()).Text();

        RuleFor(x => x.Labels).NotEmpty();
        TransformForEach(x => x.Labels, x => x?.Trim()).Text();
    }
}