using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Command.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineInteractor : IAddPendingChangeLogLine
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;

        public AddPendingChangeLogLineInteractor(IProjectDao projectDao, IChangeLogQueriesDao changeLogQueriesDao,
            IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _changeLogQueries = changeLogQueriesDao ?? throw new ArgumentNullException(nameof(changeLogQueriesDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        }

        public async Task ExecuteAsync(IAddPendingLineOutputPort output,
            PendingLineRequestModel lineRequestModel)
        {
            var project = await _projectDao.FindProjectAsync(lineRequestModel.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            _unitOfWork.Start();

            var line = await CreateChangeLogLineAsync(output, lineRequestModel, project.Value);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddPendingLineOutputPort output,
            PendingLineRequestModel requestModel,
            Project project)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

            var parsedLine = LineParser.Parse(output, lineParsingRequestModel);
            if (parsedLine.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogsMetadata = await _changeLogQueries.GetChangeLogsMetadataAsync(project.Id);

            if (!changeLogsMetadata.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<ChangeLogLine>.None;
            }

            if (changeLogsMetadata.Texts.Contains(parsedLine.Value.Text))
            {
                output.LineWithSameTextAlreadyExists(requestModel.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                null, project.Id,
                parsedLine.Value.Text, changeLogsMetadata.NextFreePosition, DateTime.UtcNow,
                parsedLine.Value.Labels, parsedLine.Value.Issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private async Task SaveChangeLogLineAsync(IAddPendingLineOutputPort outputPort, ChangeLogLine line)
        {
            await _changeLogCommands
                .AddLineAsync(line)
                .Match(Finish, c => outputPort.Conflict(c));

            void Finish(ChangeLogLine l)
            {
                _unitOfWork.Commit();
                outputPort.Created(l.Id);
            }
        }
    }
}