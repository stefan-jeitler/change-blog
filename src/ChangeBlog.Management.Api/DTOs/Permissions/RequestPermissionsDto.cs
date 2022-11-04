using System;
using FluentValidation;

namespace ChangeBlog.Management.Api.DTOs.Permissions;

public class RequestPermissionsDto
{
    public ResourceType ResourceType { get; set; }
    public Guid ResourceId { get; set; }
}

public class RequestResourcePermissionsDtoValidator : AbstractValidator<RequestPermissionsDto>
{
    public RequestResourcePermissionsDtoValidator()
    {
        RuleFor(x => x.ResourceType).IsInEnum();
        RuleFor(x => x.ResourceId).NotEmpty();
    }
}