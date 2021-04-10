using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public record PendingChangeLogLineDto(Guid ProjectId, string Text, List<string> Labels, List<string> Issues);
}
