using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public record ChangeLogLineDto(Guid ProjectId,
        string Version,
        string Text,
        List<string> Labels,
        List<string> Issues);
}