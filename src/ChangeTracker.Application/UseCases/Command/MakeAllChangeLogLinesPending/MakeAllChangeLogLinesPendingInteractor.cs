using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Command.MakeAllChangeLogLinesPending
{
    public class MakeAllChangeLogLinesPendingInteractor : IMakeAllChangeLogLinesPending
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IChangeLogQueriesDao _changeLogQueries;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVersionDao _versionDao;

        public MakeAllChangeLogLinesPendingInteractor(IVersionDao versionDao, IChangeLogQueriesDao changeLogQueries,
            IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
        {
            _versionDao = versionDao;
            _changeLogQueries = changeLogQueries;
            _changeLogCommands = changeLogCommands;
            _unitOfWork = unitOfWork;
        }

        public async Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid projectId, string version)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("projectId cannot be empty.");

            if (version is null)
                throw new ArgumentNullException(nameof(version));

            if (!ClVersionValue.TryParse(version, out var clVersionValue))
            {
                output.InvalidVersionFormat(version);
                return;
            }

            var clVersion = await _versionDao.FindVersionAsync(projectId, clVersionValue);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await MakeAllLinesPendingAsync(output, clVersion.Value);
        }

        public async Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid versionId)
        {
            if (versionId == Guid.Empty)
                throw new ArgumentException("versionId cannot be empty.");

            var clVersion = await _versionDao.FindVersionAsync(versionId);
            if (clVersion.HasNoValue)
            {
                output.VersionDoesNotExist();
                return;
            }

            await MakeAllLinesPendingAsync(output, clVersion.Value);
        }

        private static bool IsVersionReadOnly(IMakeAllChangeLogLinesPendingOutputPort output, ClVersion clVersion)
        {
            if (clVersion.IsReleased)
            {
                output.VersionAlreadyReleased();
                return true;
            }

            if (clVersion.IsDeleted)
            {
                output.VersionClosed();
                return true;
            }

            return false;
        }

        private async Task MakeAllLinesPendingAsync(IMakeAllChangeLogLinesPendingOutputPort output, ClVersion clVersion)
        {
            if (IsVersionReadOnly(output, clVersion))
                return;

            _unitOfWork.Start();

            var projectId = clVersion.ProjectId;
            var versionId = clVersion.Id;
            var pendingChangeLogsMetadata = await _changeLogQueries.GetChangeLogsMetadataAsync(projectId);
            var versionChangeLogsMetadata = await _changeLogQueries.GetChangeLogsMetadataAsync(projectId, versionId);

            if (versionChangeLogsMetadata.Count > pendingChangeLogsMetadata.RemainingPositionsToAdd)
            {
                output.TooManyPendingLines(ChangeLogsMetadata.MaxChangeLogLines);
                return;
            }

            var duplicates = pendingChangeLogsMetadata.Texts.Intersect(versionChangeLogsMetadata.Texts);
            if (duplicates.Any())
            {
                output.LineWithSameTextAlreadyExists(duplicates.Select(x => x.Value).ToList());
                return;
            }

            await SaveAssignmentsAsync(output, versionId);
        }

        private async Task SaveAssignmentsAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid vId)
        {
            await _changeLogCommands.MakeLinesPending(vId)
                .Match(Finish, c => output.Conflict(c.Reason));

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.MadePending(count);
            }
        }
    }
}