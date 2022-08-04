using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogLine;

public interface IGetPendingChangeLogLine
{
    Task ExecuteAsync(IGetPendingChangeLogLineOutputPort output,
        Guid userId, Guid changeLogLineId);
}