using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Application.UseCases.Commands.AddChangeLogLine.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.AddChangeLogLine
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

        public async Task ExecuteAsync(IAddLineOutputPort output,
            VersionChangeLogLineRequestModelRequestModel requestModelRequestModel)
        {
            if (!ClVersionValue.TryParse(requestModelRequestModel.Version, out var versionValue))
            {
                output.InvalidVersionFormat();
                return;
            }

            var version = await _versionDao.FindVersionAsync(requestModelRequestModel.ProjectId, versionValue);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            _unitOfWork.Start();
            
            await AddLineAsync(output, requestModelRequestModel, version.Value);
        }

        public async Task ExecuteAsync(IAddLineOutputPort output,
            VersionIdChangeLogLineRequestModelRequestModel requestModelRequestModel)
        {
            var version = await _versionDao.FindVersionAsync(requestModelRequestModel.VersionId);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            _unitOfWork.Start();

            await AddLineAsync(output, requestModelRequestModel, version.Value);
        }

        private async Task AddLineAsync(IAddLineOutputPort output, IChangeLogLineRequestModel requestModel,
            ClVersion clVersion)
        {
            var line = await CreateChangeLogLineAsync(output, requestModel, clVersion);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddLineOutputPort output,
            IChangeLogLineRequestModel requestModel, ClVersion clVersion)
        {
            var parsedLine = ParseLine(output, requestModel);
            if (parsedLine.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogs = await _changeLogQueries.GetChangeLogsAsync(clVersion.ProjectId, clVersion.Id);
            if (!changeLogs.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogs.MaxLines);
                return Maybe<ChangeLogLine>.None;
            }

            if (changeLogs.Lines.Any(x => string.Equals(x.Text.Value, parsedLine.Value.Text.Value, StringComparison.CurrentCultureIgnoreCase)))
            {
                output.LineWithSameTextAlreadyExists(parsedLine.Value.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                clVersion.Id, clVersion.ProjectId, parsedLine.Value.Text,
                changeLogs.NextFreePosition, DateTime.UtcNow,
                parsedLine.Value.Labels, parsedLine.Value.Issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private static Maybe<LineParserResponseModel> ParseLine(ILineParserOutput output,
            IChangeLogLineRequestModel requestModel)
        {
            var lineParsingRequestModel = new LineParserRequestModel(requestModel.Text,
                requestModel.Labels, requestModel.Issues);

            return LineParser.Parse(output, lineParsingRequestModel);
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