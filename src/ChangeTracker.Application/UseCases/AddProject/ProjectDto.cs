using System;

namespace ChangeTracker.Application.UseCases.AddProject
{
    public record ProjectDto(Guid AccountId,
        string Name,
        Guid? VersioningSchemeId);
}