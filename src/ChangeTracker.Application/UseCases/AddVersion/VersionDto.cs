using System;

namespace ChangeTracker.Application.UseCases.AddVersion
{
    public record VersionDto(Guid ProjectId, string Version);
}