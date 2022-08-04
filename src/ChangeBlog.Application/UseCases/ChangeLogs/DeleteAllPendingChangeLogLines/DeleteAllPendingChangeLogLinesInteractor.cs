using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;

namespace ChangeBlog.Application.UseCases.ChangeLogs.DeleteAllPendingChangeLogLines;

public class DeleteAllPendingChangeLogLinesInteractor : IDeleteAllPendingChangeLogLines
{
    private readonly IChangeLogCommandsDao _changeLogCommands;

    public DeleteAllPendingChangeLogLinesInteractor(IChangeLogCommandsDao changeLogCommands)
    {
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }


    public Task ExecuteAsync(Guid productId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty.");
        }

        return _changeLogCommands.DeletePendingChangeLogs(productId);
    }
}