using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine
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
            _unitOfWork.Start();

            var project = await _projectDao.FindProjectAsync(lineRequestModel.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            var line = await CreateChangeLogLineAsync(output, lineRequestModel, project.Value);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddPendingLineOutputPort output,
            PendingLineRequestModel requestModel,
            Project project)
        {
            var parsedLine = ParseLine(output, requestModel);
            if (parsedLine.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogs = await _changeLogQueries.GetChangeLogsAsync(project.Id);

            if (!changeLogs.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogs.MaxLines);
                return Maybe<ChangeLogLine>.None;
            }

            if (changeLogs.ContainsText(parsedLine.Value.Text))
            {
                output.LineWithSameTextAlreadyExists(parsedLine.Value.Text);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                null, project.Id,
                parsedLine.Value.Text, changeLogs.NextFreePosition, DateTime.UtcNow,
                parsedLine.Value.Labels, parsedLine.Value.Issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }

        private static Maybe<LineParserResponseModel> ParseLine(ILineParserOutput output,
            PendingLineRequestModel requestModel)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);

            return LineParser.Parse(output, lineParsingRequestModel);
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