using System;

namespace ChangeTracker.Application.UseCases.CreateProject
{
    public record CreateProjectDto(Guid AccountId,
        string Name,
        Guid? VersioningSchemeId);
}