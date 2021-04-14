using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.Services.ChangeLog
{
    public record ChangeLogLineParsingDto(Guid ProjectId,
        Guid? VersionId,
        string Text,
        IList<string> Labels,
        IList<string> Issues,
        uint? Position = null);
}