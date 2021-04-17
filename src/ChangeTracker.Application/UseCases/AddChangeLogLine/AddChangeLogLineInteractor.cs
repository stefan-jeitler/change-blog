using System;
using System.Threading.Tasks;
using ChangeTracker.Application.ChangeLogLineParsing;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public class AddChangeLogLineInteractor : IAddChangeLogLine
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddChangeLogLineInteractor(IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork, IVersionDao versionDao, IProjectDao projectDao)
        {
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
        }

        public async Task ExecuteAsync(IAddLineOutputPort output, ChangeLogLineRequestModel requestModel)
        {
            if (!ClVersionValue.TryParse(requestModel.Version, out var versionValue))
            {
                output.InvalidVersionFormat();
                return;
            }

            var project = await _projectDao.FindAsync(requestModel.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            var version = await GetVersionAsync(output, project.Value.Id, versionValue);
            if (version.HasNoValue)
                return;

            _unitOfWork.Start();

            var line = await CreateChangeLogLineAsync(output, requestModel, project.Value, version.Value.Id);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ClVersion>> GetVersionAsync(IAddLineOutputPort output, Guid projectId,
            ClVersionValue versionValue)
        {
            var version = await _versionDao.FindAsync(projectId, versionValue);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist(versionValue.Value);
                return Maybe<ClVersion>.None;
            }

            if (version.Value.IsReleased)
            {
                output.VersionAlreadyReleased(versionValue);
                return Maybe<ClVersion>.None;
            }

            if (version.Value.IsDeleted)
            {
                output.VersionDeleted(versionValue);
                return Maybe<ClVersion>.None;
            }

            return version;
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IAddLineOutputPort output,
            ChangeLogLineRequestModel requestModel, Project project, Guid versionId)
        {
            var lineParsingRequestModel =
                new LineParserRequestModel(requestModel.Text, requestModel.Labels, requestModel.Issues);
            var parsedLine = LineParser.Parse(output, lineParsingRequestModel);
            if (parsedLine.HasNoValue)
                return Maybe<ChangeLogLine>.None;

            var changeLogsMetadata = await _changeLogDao.GetChangeLogsMetadataAsync(project.Id, versionId);

            if (!changeLogsMetadata.IsPositionAvailable)
            {
                output.TooManyLines(ChangeLogsMetadata.MaxChangeLogLines);
                return Maybe<ChangeLogLine>.None;
            }

            var changeLogLine = new ChangeLogLine(Guid.NewGuid(),
                versionId, project.Id,
                parsedLine.Value.Text, changeLogsMetadata.NextFreePosition, DateTime.UtcNow,
                parsedLine.Value.Labels, parsedLine.Value.Issues);

            return Maybe<ChangeLogLine>.From(changeLogLine);
        }


        private async Task SaveChangeLogLineAsync(IAddLineOutputPort output, ChangeLogLine changeLogLine)
        {
            await _changeLogDao
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