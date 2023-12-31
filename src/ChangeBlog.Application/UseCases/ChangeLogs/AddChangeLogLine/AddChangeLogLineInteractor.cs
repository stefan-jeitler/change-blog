using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Versions;
using ChangeBlog.Application.ChangeLogLineParser;
using ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine.Models;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine;

public class AddChangeLogLineInteractor : IAddChangeLogLine
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IBusinessTransaction _businessTransaction;
    private readonly IVersionDao _versionDao;

    public AddChangeLogLineInteractor(IChangeLogCommandsDao changeLogCommands,
        IChangeLogQueriesDao changeLogQueriesDao, IBusinessTransaction businessTransaction, IVersionDao versionDao)
    {
        _changeLogQueries = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
        _businessTransaction = businessTransaction ?? throw new ArgumentNullException(nameof(businessTransaction));
        _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }

    public async Task ExecuteAsync(IAddChangeLogLineOutputPort output,
        VersionChangeLogLineRequestModel requestModel)
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

        await AddLineAsync(output, requestModel, version.GetValueOrThrow());
    }

    public async Task ExecuteAsync(IAddChangeLogLineOutputPort output,
        VersionIdChangeLogLineRequestModel requestModel)
    {
        var version = await _versionDao.FindVersionAsync(requestModel.VersionId);
        if (version.HasNoValue)
        {
            output.VersionDoesNotExist();
            return;
        }

        _businessTransaction.Start();

        await AddLineAsync(output, requestModel, version.GetValueOrThrow());
    }

    private async Task AddLineAsync(IAddChangeLogLineOutputPort output, IChangeLogLineRequestModel requestModel,
        ClVersion clVersion)
    {
        var line = await CreateChangeLogLineAsync(output, requestModel, clVersion);
        if (line.HasNoValue)
            return;

        await SaveChangeLogLineAsync(output, line.GetValueOrThrow());
    }

    private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddChangeLogLineOutputPort output,
        IChangeLogLineRequestModel requestModel, ClVersion clVersion)
    {
        var parsedLine = ParseLine(output, requestModel);
        if (parsedLine.HasNoValue) return Maybe<ChangeLogLine>.None;

        var changeLogs = await _changeLogQueries.GetChangeLogsAsync(clVersion.ProductId, clVersion.Id);
        if (!changeLogs.IsPositionAvailable)
        {
            output.TooManyLines(Domain.ChangeLog.ChangeLogs.MaxLines);
            return Maybe<ChangeLogLine>.None;
        }

        var existingLineWithSameText =
            changeLogs.Lines.FirstOrDefault(x => x.Text.Equals(parsedLine.GetValueOrThrow().Text));
        if (existingLineWithSameText is not null)
        {
            output.DuplicateEntry(existingLineWithSameText.Id, parsedLine.GetValueOrThrow().Text);
            return Maybe<ChangeLogLine>.None;
        }

        var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
            clVersion.Id, clVersion.ProductId, parsedLine.GetValueOrThrow().Text,
            changeLogs.NextFreePosition, DateTime.UtcNow,
            parsedLine.GetValueOrThrow().Labels, parsedLine.GetValueOrThrow().Issues, requestModel.UserId);

        return Maybe<ChangeLogLine>.From(changeLogLine);
    }

    private static Maybe<LineParserResponseModel> ParseLine(ILineParserOutput output,
        IChangeLogLineRequestModel requestModel)
    {
        var lineParsingRequestModel = new LineParserRequestModel(requestModel.Text,
            requestModel.Labels, requestModel.Issues);

        return LineParser.Parse(output, lineParsingRequestModel);
    }

    private async Task SaveChangeLogLineAsync(IAddChangeLogLineOutputPort output, ChangeLogLine changeLogLine)
    {
        await _changeLogCommands
            .AddOrUpdateLineAsync(changeLogLine)
            .Match(Finish, output.Conflict);

        void Finish(ChangeLogLine l)
        {
            _businessTransaction.Commit();
            output.Created(l.Id);
        }
    }
}