using System;

namespace ChangeTracker.Application.UseCases.CreateVersion
{
    public record CreateVersionDto(Guid ProjectId, string Version);
}