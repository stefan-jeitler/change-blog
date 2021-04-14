using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddCompleteVersion.DTOs
{
    public record CompleteVersionDto(Guid ProjectId, string Version, List<ChangeLogLineDto> Lines,
        bool ReleaseImmediately = false);
}