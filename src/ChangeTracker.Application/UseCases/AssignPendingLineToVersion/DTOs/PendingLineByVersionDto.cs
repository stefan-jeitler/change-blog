using System;

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion.DTOs
{
    public record PendingLineByVersionDto(Guid ProjectId, string Version, Guid ChangeLogLineId);
}
