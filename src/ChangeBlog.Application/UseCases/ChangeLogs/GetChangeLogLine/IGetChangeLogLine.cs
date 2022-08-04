using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetChangeLogLine;

public interface IGetChangeLogLine
{
    Task ExecuteAsync(IGetChangeLogLineOutputPort output, Guid userId, Guid changeLogLineId);
}