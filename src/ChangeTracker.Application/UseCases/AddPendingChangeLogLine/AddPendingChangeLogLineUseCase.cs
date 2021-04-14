using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLog;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineUseCase : IAddPendingChangeLogLineUseCase
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly ChangeLogLineParsingService _changeLogLineParsing;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;

        public AddPendingChangeLogLineUseCase(IProjectDao projectDao, IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork, ChangeLogLineParsingService changeLogLineParsing)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogLineParsing =
                changeLogLineParsing ?? throw new ArgumentNullException(nameof(changeLogLineParsing));
        }

        public async Task ExecuteAsync(IAddPendingChangeLogLineOutputPort output,
            PendingChangeLogLineDto changeLogLineDto)
        {
            var project = await _projectDao.FindAsync(changeLogLineDto.ProjectId);
            if (project.HasNoValue)
            {
                output.ProjectDoesNotExist();
                return;
            }

            _unitOfWork.Start();

            var line = await CreateChangeLogLineAsync(output, changeLogLineDto, project.Value);
            if (line.HasNoValue)
                return;

            await SaveChangeLogLineAsync(output, line.Value);
        }

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IChangeLogLineParsingOutput output,
            PendingChangeLogLineDto changeLogLineDto,
            Project project)
        {
            var parsingDto = new ChangeLogLineParsingDto(project.Id,
                null, changeLogLineDto.Text,
                changeLogLineDto.Labels, changeLogLineDto.Issues);

            return await _changeLogLineParsing.ParseAsync(output, parsingDto);
        }


        private async Task SaveChangeLogLineAsync(IAddPendingChangeLogLineOutputPort outputPort, ChangeLogLine line)
        {
            await _changeLogDao
                .AddLineAsync(line)
                .Match(Finish, c => outputPort.Conflict(c));

            void Finish(ChangeLogLine l)
            {
                outputPort.Created(l.Id);
                _unitOfWork.Commit();
            }
        }
    }
}