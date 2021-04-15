using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.Services.ChangeLogLineParsing;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineInteractor : IAddPendingChangeLogLine
    {
        private readonly IChangeLogDao _changeLogDao;
        private readonly ChangeLogLineParsingService _changeLogLineParsing;
        private readonly IProjectDao _projectDao;
        private readonly IUnitOfWork _unitOfWork;

        public AddPendingChangeLogLineInteractor(IProjectDao projectDao, IChangeLogDao changeLogDao,
            IUnitOfWork unitOfWork, ChangeLogLineParsingService changeLogLineParsing)
        {
            _projectDao = projectDao ?? throw new ArgumentNullException(nameof(projectDao));
            _changeLogDao = changeLogDao ?? throw new ArgumentNullException(nameof(changeLogDao));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _changeLogLineParsing = changeLogLineParsing ?? throw new ArgumentNullException(nameof(changeLogLineParsing));
        }

        public async Task ExecuteAsync(IAddPendingChangeLogLineOutputPort output,
            PendingLineRequestModel lineRequestModel)
        {
            var project = await _projectDao.FindAsync(lineRequestModel.ProjectId);
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

        private async Task<Maybe<ChangeLogLine>> CreateChangeLogLineAsync(IChangeLogLineParsingOutput output,
            PendingLineRequestModel lineRequestModel,
            Project project)
        {
            var lineParsingRequestModel = new LineParsingRequestModel(project.Id,
                null, lineRequestModel.Text,
                lineRequestModel.Labels, lineRequestModel.Issues);

            return await _changeLogLineParsing.ParseAsync(output, lineParsingRequestModel);
        }


        private async Task SaveChangeLogLineAsync(IAddPendingChangeLogLineOutputPort outputPort, ChangeLogLine line)
        {
            await _changeLogDao
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