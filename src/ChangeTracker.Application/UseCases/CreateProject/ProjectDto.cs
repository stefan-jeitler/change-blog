using System;

namespace ChangeTracker.Application.UseCases.CreateProject
{
    public record ProjectDto(Guid AccountId,
        string Name,
        Guid? VersioningSchemeId);
}