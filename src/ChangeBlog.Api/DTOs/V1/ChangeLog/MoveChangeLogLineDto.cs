using System;
using FluentValidation;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog;

public class MoveChangeLogLineDto
{
    public Guid TargetVersionId { get; set; }
}

public class MoveChangeLogLineDtoValidator : AbstractValidator<MoveChangeLogLineDto>
{
    public MoveChangeLogLineDtoValidator()
    {
        RuleFor(x => x.TargetVersionId).NotEmpty();
    }
}