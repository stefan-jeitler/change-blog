using System;

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion.DTOs
{
    public record PendingLineByVersionIdDto(Guid ProjectId, Guid VersionId, Guid ChangeLogLineId);
}