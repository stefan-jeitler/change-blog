using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable SwitchStatementHandlesSomeKnownEnumValuesWithDefault

namespace ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;

public class DeleteChangeLogLineInteractor : IDeleteChangeLogLine
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;

    public DeleteChangeLogLineInteractor(IChangeLogCommandsDao changeLogCommands,
        IChangeLogQueriesDao changeLogQueries)
    {
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
    }

    public async Task ExecuteAsync(IDeleteChangeLogLineOutputPort output,
        DeleteChangeLogLineRequestModel requestModel)
    {
        var existingLine = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);

        if (existingLine.HasNoValue)
        {
            output.LineDoesNotExist(requestModel.ChangeLogLineId);
            return;
        }

        var changeLogLine = existingLine.GetValueOrThrow();

        switch (requestModel.ChangeLogLineType)
        {
            case ChangeLogLineType.Pending when !changeLogLine.IsPending:
                output.RequestedLineIsNotPending(changeLogLine.Id);
                return;
            case ChangeLogLineType.NotPending when changeLogLine.IsPending:
                output.RequestedLineIsPending(changeLogLine.Id);
                return;
        }

        if (changeLogLine.DeletedAt.HasValue)
        {
            output.LineDeleted(changeLogLine.Id);
            return;
        }

        await _changeLogCommands.DeleteLineAsync(changeLogLine)
            .Match(l => output.LineDeleted(l.Id), output.Conflict);
    }
}