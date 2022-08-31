using System;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Versions;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeBlog.Application.UseCases.ChangeLogs.MakeChangeLogLinePending;

public class MakeChangeLogLinePendingInteractor : IMakeChangeLogLinePending
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IVersionDao _versionDao;

    public MakeChangeLogLinePendingInteractor(IVersionDao versionDao, IChangeLogQueriesDao changeLogQueries,
        IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
    {
        _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }

    public async Task ExecuteAsync(IMakeChangeLogLinePendingOutputPort output, Guid changeLogLineId)
    {
        Guard.Against.NullOrEmpty(changeLogLineId, nameof(changeLogLineId));

        _unitOfWork.Start();

        var line = await GetLineAsync(output, changeLogLineId);
        if (line.HasNoValue)
        {
            return;
        }

        var clVersion = await GetVersionAsync(output, line.GetValueOrThrow());
        if (clVersion.HasNoValue)
        {
            return;
        }

        var pendingChangeLogMetadata = await GetChangeLogsAsync(output, line.GetValueOrThrow());
        if (pendingChangeLogMetadata.HasNoValue)
        {
            return;
        }

        var pendingLine = MakeLinePending(line.GetValueOrThrow(), pendingChangeLogMetadata.GetValueOrThrow());
        await MoveLineAsyncAsync(output, pendingLine);
    }

    private async Task<Maybe<ChangeLogLine>> GetLineAsync(IMakeChangeLogLinePendingOutputPort output,
        Guid changeLogLineId)
    {
        var line = await _changeLogQueries.FindLineAsync(changeLogLineId);
        if (line.HasNoValue)
        {
            output.ChangeLogLineDoesNotExist();
            return Maybe<ChangeLogLine>.None;
        }

        if (line.GetValueOrThrow().IsPending)
        {
            output.ChangeLogLineIsAlreadyPending(line.GetValueOrThrow().Id);
            return Maybe<ChangeLogLine>.None;
        }

        return line;
    }

    private async Task<Maybe<ClVersion>> GetVersionAsync(IMakeChangeLogLinePendingOutputPort output,
        ChangeLogLine line)
    {
        var versionId = line.VersionId!.Value;
        var clVersion = await _versionDao.GetVersionAsync(versionId);

        if (clVersion.IsReleased)
        {
            output.VersionAlreadyReleased(versionId);
            return Maybe<ClVersion>.None;
        }

        if (clVersion.IsDeleted)
        {
            output.VersionDeleted(versionId);
            return Maybe<ClVersion>.None;
        }

        return Maybe<ClVersion>.From(clVersion);
    }

    private async Task<Maybe<Domain.ChangeLog.ChangeLogs>> GetChangeLogsAsync(
        IMakeChangeLogLinePendingOutputPort output,
        ChangeLogLine line)
    {
        var pendingChangeLogs = await _changeLogQueries.GetChangeLogsAsync(line.ProductId);
        if (!pendingChangeLogs.IsPositionAvailable)
        {
            output.TooManyPendingLines(Domain.ChangeLog.ChangeLogs.MaxLines);
            return Maybe<Domain.ChangeLog.ChangeLogs>.None;
        }

        var existingLineWithSameText = pendingChangeLogs.Lines.FirstOrDefault(x => x.Text.Equals(line.Text));
        if (existingLineWithSameText is not null)
        {
            output.LineWithSameTextAlreadyExists(existingLineWithSameText.Id, line.Text);
            return Maybe<Domain.ChangeLog.ChangeLogs>.None;
        }

        return Maybe<Domain.ChangeLog.ChangeLogs>.From(pendingChangeLogs);
    }

    private static ChangeLogLine MakeLinePending(ChangeLogLine line, Domain.ChangeLog.ChangeLogs pendingChangeLogs)
    {
        return new ChangeLogLine(line.Id,
            null,
            line.ProductId,
            line.Text,
            pendingChangeLogs.NextFreePosition,
            line.CreatedAt,
            line.Labels,
            line.Issues,
            line.CreatedByUser,
            line.DeletedAt);
    }

    private async Task MoveLineAsyncAsync(IMakeChangeLogLinePendingOutputPort output, ChangeLogLine line)
    {
        await _changeLogCommands.MoveLineAsync(line)
            .Match(Finish, output.Conflict);

        void Finish(ChangeLogLine m)
        {
            _unitOfWork.Commit();
            output.WasMadePending(m.Id);
        }
    }
}