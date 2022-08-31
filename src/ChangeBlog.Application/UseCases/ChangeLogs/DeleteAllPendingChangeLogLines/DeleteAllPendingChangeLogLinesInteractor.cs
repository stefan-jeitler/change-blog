using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
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
        Guard.Against.NullOrEmpty(productId, nameof(productId));

        return _changeLogCommands.DeletePendingChangeLogs(productId);
    }
}