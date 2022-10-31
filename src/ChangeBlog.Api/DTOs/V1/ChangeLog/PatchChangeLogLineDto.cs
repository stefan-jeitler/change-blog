using System.Collections.Generic;
using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog;

public class PatchChangeLogLineDto
{
    public string Text { get; set; }
    public List<string> Labels { get; set; }
    public List<string> Issues { get; set; }
}

public class PatchChangeLogLineDtoValidator : AbstractValidator<PatchChangeLogLineDto>
{
    public PatchChangeLogLineDtoValidator()
    {
        RuleFor(x => x.Text)
            .MustBeChangeLogText()
            .When(x => !string.IsNullOrEmpty(x.Text));

        RuleForEach(x => x.Labels)
            .MustBeLabel()
            .When(x => x.Labels is not null);

        RuleForEach(x => x.Issues)
            .MustBeIssue()
            .When(x => x.Issues is not null);
    }
}