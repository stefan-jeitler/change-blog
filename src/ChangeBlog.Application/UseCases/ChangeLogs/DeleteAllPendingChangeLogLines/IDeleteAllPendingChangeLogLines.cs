using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.ChangeLogs.DeleteAllPendingChangeLogLines;

public interface IDeleteAllPendingChangeLogLines
{
    Task ExecuteAsync(Guid productId);
}