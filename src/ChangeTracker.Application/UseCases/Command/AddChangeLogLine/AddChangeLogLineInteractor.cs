using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Application.UseCases.Command.AddChangeLogLine.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Command.AddChangeLogLine
{
    public class AddChangeLogLineInteractor : IAddChangeLogLine
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddChangeLogLineInteractor(IChangeLogCommandsDao changeLogCommands,
            IChangeLogQueriesDao changeLogQueriesDao, IUnitOfWork unitOfWork, IVersionDao versionDao)
        {
            _changeLogQueries = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAddLineOutputPort output, VersionChangeLogLineRequestModel requestModel)
        {
            if (!ClVersionValue.TryParse(requestModel.Version, out var versionValue))
            {
                output.InvalidVersionFormat();
                return;
            }

            var version = await _versionDao.FindVersionAsync(requestModel.ProjectId, versionValue);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            _unitOfWork.Start();

            await AddLineAsync(output, requestModel, version.Value);
        }

        public async Task ExecuteAsync(IAddLineOutputPort output, VersionIdChangeLogLineRequestModel requestModel)
        {
            var version = await _versionDao.FindVersionAsync(requestModel.VersionId);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            _unitOfWork.Start();

            await AddLineAsync(output, requestModel, version.Value);
        }

        private async Task AddLineAsync(IAddLineOutputPort output, IChangeLogLine requestModel,
            ClVersion clVersion)
        {
            var line = await CreateChangeLogLineAsync(output, requestModel, clVersion);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddLineOutputPort output,
            IChangeLogLine requestModel, ClVersion version)
        {
            var lineParsingRequestModel = new LineParserRequestModel(requestModel.Text,
                requestModel.Labels, requestModel.Issues);

            var parsedLine = LineParser.Parse(output, lineParsingRequestModel);
            if (parsedLine.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogsMetadata = await GetChangeLogsMetadataAsync(output, version, parsedLine.Value.Text);
            if (changeLogsMetadata.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                version.Id, version.ProjectId, parsedLine.Value.Text,
                changeLogsMetadata.Value.NextFreePosition, DateTime.UtcNow,
                parsedLine.Value.Labels, parsedLine.Value.Issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task<Maybe<ChangeLogsMetadata>> GetChangeLogsMetadataAsync(IAddLineOutputPort output,
            ClVersion clVersion, ChangeLogText text)
        {
            var changeLogsMetadata =
                await _changeLogQueries.GetChangeLogsMetadataAsync(clVersion.ProjectId, clVersion.Id);
            if (!changeLogsMetadata.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<ChangeLogsMetadata>.None;
            }

            if (changeLogsMetadata.Texts.Contains(text))
            {
                output.LineWithSameTextAlreadyExists(text);
                return Maybe<ChangeLogsMetadata>.None;
            }

            return Maybe<ChangeLogsMetadata>.From(changeLogsMetadata);
        }

        private async Task SaveChangeLogLineAsync(IAddLineOutputPort output, ChangeLogLine changeLogLine)
        {
            await _changeLogCommands
                .AddLineAsync(changeLogLine)
                .Match(Finish, c => output.Conflict(c));

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                output.Created(l.Id);
            }
        }
    }
}