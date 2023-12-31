using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.UseCases.ChangeLogs.Labels.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Labels.DeleteChangeLogLineLabel;

public class DeleteChangeLogLineLabelInteractor : IDeleteChangeLogLineLabel
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IBusinessTransaction _businessTransaction;

    public DeleteChangeLogLineLabelInteractor(IBusinessTransaction businessTransaction, IChangeLogQueriesDao changeLogQueries,
        IChangeLogCommandsDao changeLogCommands)
    {
        _businessTransaction = businessTransaction ?? throw new ArgumentNullException(nameof(businessTransaction));
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }

    public async Task ExecuteAsync(IDeleteChangeLogLineLabelOutputPort output,
        ChangeLogLineLabelRequestModel requestModel)
    {
        if (!Label.TryParse(requestModel.Label, out var label))
        {
            output.InvalidLabel(requestModel.Label);
            return;
        }

        _businessTransaction.Start();

        var line = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);
        if (line.HasNoValue)
        {
            output.ChangeLogLineDoesNotExist();
            return;
        }

        await RemoveLabelAsync(output, line.GetValueOrThrow(), label);
    }

    private async Task RemoveLabelAsync(IDeleteChangeLogLineLabelOutputPort output, ChangeLogLine line, Label label)
    {
        line.RemoveLabel(label);

        await _changeLogCommands.UpdateLineAsync(line)
            .Match(Finish, output.Conflict);

        void Finish(ChangeLogLine l)
        {
            _businessTransaction.Commit();
            output.Deleted(l.Id);
        }
    }
}