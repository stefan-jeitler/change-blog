using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLog;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public class AddChangeLogLineUseCase : IAddChangeLogLineUseCase
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly ChangeLogLineParsingService _changeLogLineParsing;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AddChangeLogLineUseCase(IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork, IVersionDao versionDao, IProjectDao projectDao,
            ChangeLogLineParsingService changeLogLineParsing)
        {
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _versionDao = versionDao;
            _projectDao = projectDao;
            _changeLogLineParsing = changeLogLineParsing;
        }

        public async Task ExecuteAsync(IAddChangeLogLineOutputPort output, ChangeLogLineDto changeLogLineDto)
        {
            if (!ClVersionValue.TryParse(changeLogLineDto.Version, out var versionValue))
            {
                output.InvalidVersionFormat();
                return;
            }

            var project = await _projectDao.FindAsync(changeLogLineDto.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            var version = await _versionDao.FindAsync(project.Value.Id, versionValue);
            if (version.HasNoValue)
            {
                output.VersionDoesNotExist(versionValue.Value);
                return;
            }

            _unitOfWork.Start();

            var line = await CreateChangeLogLineAsync(output, changeLogLineDto, project.Value, version.Value.Id);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IChangeLogLineParsingOutput output,
            ChangeLogLineDto changeLogLineDto, Project project, Guid versionId)
        {
            var parsingDto = new ChangeLogLineParsingDto(project.Id,
                versionId, changeLogLineDto.Text,
                changeLogLineDto.Labels, changeLogLineDto.Issues);

            return await _changeLogLineParsing.ParseAsync(output, parsingDto);
        }

        private async Task SaveChangeLogLineAsync(IAddChangeLogLineOutputPort output, ChangeLogLine changeLogLine)
        {
            await _changeLogDao
                .AddLineAsync(changeLogLine)
                .Match(Finish, c => output.Conflict(c));

            void Finish(ChangeLogLine l)
            {
                output.Created(l.Id);
                _unitOfWork.Commit();
            }
        }
    }
}