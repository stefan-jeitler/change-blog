using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.MakeChangeLogLinePending;

public interface IMakeChangeLogLinePending
{
    Task ExecuteAsync(IMakeChangeLogLinePendingOutputPort output, Guid changeLogLineId);
}