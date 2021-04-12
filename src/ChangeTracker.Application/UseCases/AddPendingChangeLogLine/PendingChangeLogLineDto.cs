using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public record PendingChangeLogLineDto(Guid ProjectId, string Text, List<string> Labels, List<string> Issues);
}