using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public record AddPendingChangeLogLineDto(Guid ProjectId, string Text, List<string> Labels);
}