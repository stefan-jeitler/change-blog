using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Queries.GetChangeLogLine;

public interface IGetChangeLogLine
{
    Task ExecuteAsync(IGetChangeLogLineOutputPort output, Guid userId, Guid changeLogLineId);
}