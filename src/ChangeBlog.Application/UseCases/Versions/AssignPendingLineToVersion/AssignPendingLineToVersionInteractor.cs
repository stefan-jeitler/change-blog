using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Versions;
using ChangeBlog.Application.UseCases.Versions.AssignPendingLineToVersion.Models;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable UseDeconstructionOnParameter

namespace ChangeBlog.Application.UseCases.Versions.AssignPendingLineToVersion;

public class AssignPendingLineToVersionInteractor : IAssignPendingLineToVersion
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IBusinessTransaction _businessTransaction;
    private readonly IVersionDao _versionDao;

    public AssignPendingLineToVersionInteractor(IVersionDao versionDao, IChangeLogQueriesDao changeLogQueriesDao,
        IChangeLogCommandsDao changeLogCommands, IBusinessTransaction businessTransaction)
    {
        _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
        _changeLogQueries = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
        _businessTransaction = businessTransaction ?? throw new ArgumentNullException(nameof(businessTransaction));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }

    public async Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output,
        VersionIdAssignmentRequestModel requestModel)
    {
        _businessTransaction.Start();

        var version = await _versionDao.FindVersionAsync(requestModel.VersionId);
        if (version.HasNoValue)
        {
            output.VersionDoesNotExist();
            return;
        }

        await AssignToVersionAsync(output, version.GetValueOrThrow(), requestModel.ChangeLogLineId);
    }

    public async Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output,
        VersionAssignmentRequestModel requestModel)
    {
        if (!ClVersionValue.TryParse(requestModel.Version, out var versionValue))
        {
            output.InvalidVersionFormat(requestModel.Version);
            return;
        }

        _businessTransaction.Start();

        var version = await _versionDao.FindVersionAsync(requestModel.ProductId, versionValue);
        if (version.HasNoValue)
        {
            output.VersionDoesNotExist();
            return;
        }

        await AssignToVersionAsync(output, version.GetValueOrThrow(), requestModel.ChangeLogLineId);
    }

    private async Task AssignToVersionAsync(IAssignPendingLineToVersionOutputPort output, ClVersion version,
        Guid pendingLineId)
    {
        var changeLogs = await _changeLogQueries.GetChangeLogsAsync(version.ProductId, version.Id);
        if (!changeLogs.IsPositionAvailable)
        {
            output.MaxChangeLogLinesReached(Domain.ChangeLog.ChangeLogs.MaxLines);
            return;
        }

        var pendingLine = await _changeLogQueries.FindLineAsync(pendingLineId);
        if (pendingLine.HasNoValue)
        {
            output.ChangeLogLineDoesNotExist(pendingLineId);
            return;
        }

        if (pendingLine.GetValueOrThrow().ProductId != version.ProductId)
        {
            output.TargetVersionBelongsToDifferentProduct(pendingLine.GetValueOrThrow().ProductId, version.ProductId);
            return;
        }

        if (!pendingLine.GetValueOrThrow().IsPending)
        {
            output.ChangeLogLineIsNotPending(pendingLine.GetValueOrThrow().Id);
            return;
        }

        if (changeLogs.ContainsText(pendingLine.GetValueOrThrow().Text))
        {
            output.LineWithSameTextAlreadyExists(pendingLine.GetValueOrThrow().Text);
            return;
        }

        var assignedLine = pendingLine.GetValueOrThrow().AssignToVersion(version.Id, changeLogs.NextFreePosition);
        await SaveAssignmentAsync(output, assignedLine);
    }

    private async Task SaveAssignmentAsync(IAssignPendingLineToVersionOutputPort output, ChangeLogLine assignedLine)
    {
        await _changeLogCommands.MoveLineAsync(assignedLine)
            .Match(Finish, output.Conflict);

        void Finish(ChangeLogLine l)
        {
            _businessTransaction.Commit();
            output.Assigned(l.VersionId!.Value, l.Id);
        }
    }
}