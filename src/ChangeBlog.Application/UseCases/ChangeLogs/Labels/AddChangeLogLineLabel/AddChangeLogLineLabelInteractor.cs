using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.UseCases.ChangeLogs.Labels.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeBlog.Application.UseCases.ChangeLogs.Labels.AddChangeLogLineLabel;

public class AddChangeLogLineLabelInteractor : IAddChangeLogLineLabel
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IUnitOfWork _unitOfWork;

    public AddChangeLogLineLabelInteractor(IUnitOfWork unitOfWork, IChangeLogQueriesDao changeLogQueries,
        IChangeLogCommandsDao changeLogCommands)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }

    public async Task ExecuteAsync(IAddChangeLogLineLabelOutputPort output,
        ChangeLogLineLabelRequestModel requestModel)
    {
        if (!Label.TryParse(requestModel.Label, out var label))
        {
            output.InvalidLabel(requestModel.Label);
            return;
        }

        _unitOfWork.Start();

        var line = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);
        if (line.HasNoValue)
        {
            output.ChangeLogLineDoesNotExist();
            return;
        }

        if (line.GetValueOrThrow().AvailableLabelPlaces <= 0)
        {
            output.MaxLabelsReached(ChangeLogLine.MaxLabels);
            return;
        }

        await AddLabelAsync(output, line.GetValueOrThrow(), label);
    }

    private async Task AddLabelAsync(IAddChangeLogLineLabelOutputPort output, ChangeLogLine line, Label label)
    {
        line.AddLabel(label);

        await _changeLogCommands.UpdateLineAsync(line)
            .Match(Finish, output.Conflict);

        void Finish(ChangeLogLine l)
        {
            _unitOfWork.Commit();
            output.Added(l.Id);
        }
    }
}