using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public record AddChangeLogLineDto(Guid ProjectId,
        string Version,
        string Text,
        List<string> Labels,
        List<string> Issues);
}