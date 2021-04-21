using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion.Models;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion
{
    public class AssignAllPendingLinesToVersionInteractor : IAssignAllPendingLinesToVersion
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public AssignAllPendingLinesToVersionInteractor(IVersionDao versionDao, IUnitOfWork unitOfWork,
            IChangeLogCommandsDao changeLogCommands, IChangeLogQueriesDao changeLogQueries)
        {
            _versionDao = versionDao;
            _unitOfWork = unitOfWork;
            _changeLogCommands = changeLogCommands;
            _changeLogQueries = changeLogQueries;
        }

        public async Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
            VersionIdAssignmentRequestModel requestModel)
        {
        }

        public async Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
            VersionAssignmentRequestModel requestModel)
        {
            var projectId = requestModel.ProjectId;

            if (!ClVersionValue.TryParse(requestModel.Version, out var version))
            {
                output.InvalidVersionFormat(requestModel.Version);
                return;
            }

            _unitOfWork.Start();

            var clVersion = await _versionDao.FindVersionAsync(projectId, version);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            var versionChangeLogs = await _changeLogQueries.GetChangeLogsMetadataAsync(projectId, clVersion.Value.Id);
            var pendingChangeLogs = await _changeLogQueries.GetChangeLogsMetadataAsync(projectId);

            if (pendingChangeLogs.Count > versionChangeLogs.RemainingPositionsToAdd)
            {
                output.TooManyLinesToAdd(versionChangeLogs.RemainingPositionsToAdd);
                return;
            }

            var lines = await _changeLogQueries.GetPendingLines(projectId);

            var assignedLines = lines
                .Select((x, i) => new {Line = x, Position = versionChangeLogs.NextFreePosition + i})
                .Select(x => x.Line.AssignToVersion(clVersion.Value.Id, (uint) x.Position));

            await _changeLogCommands.MoveLinesAsync(assignedLines)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.Assigned(clVersion.Value.Id);
            }
        }
    }
}