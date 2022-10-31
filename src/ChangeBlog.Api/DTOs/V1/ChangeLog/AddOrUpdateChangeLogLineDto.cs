using System.Collections.Generic;
using ChangeBlog.Api.Shared.ValidatorExtensions;
using FluentValidation;
using JetBrains.Annotations;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog;

public class AddOrUpdateChangeLogLineDto
{
    public string Text { get; set; }

    public List<string> Labels { get; set; }

    public List<string> Issues { get; set; }
}

[UsedImplicitly]
public class AddOrUpdateChangeLogLineDtoValidator : AbstractValidator<AddOrUpdateChangeLogLineDto>
{
    public AddOrUpdateChangeLogLineDtoValidator()
    {
        RuleFor(x => x.Text).MustBeChangeLogText();

        RuleFor(x => x.Issues).NotNull();
        RuleForEach(x => x.Issues)
            .MustBeIssue();

        RuleFor(x => x.Labels).NotNull();
        RuleForEach(x => x.Labels)
            .MustBeLabel();
    }
}