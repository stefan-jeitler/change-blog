using System;

namespace ChangeTracker.Application.UseCases.CreateVersion
{
    public record VersionDto(Guid ProjectId, string Version);
}