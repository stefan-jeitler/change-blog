using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

// ReSharper disable InvertIf

namespace ChangeTracker.Application.UseCases.Commands.MakeAllChangeLogLinesPending
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
            _versionDao = versionDao ?? throw new ArgumentNullException(nameof(versionDao));
            _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
            _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task ExecuteAsync(IMakeAllChangeLogLinesPendingOutputPort output, Guid productId, string version)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("productId cannot be empty.");

            if (version is null)
                throw new ArgumentNullException(nameof(version));

            if (!ClVersionValue.TryParse(version, out var clVersionValue))
            {
                output.InvalidVersionFormat(version);
                return;
            }

            var clVersion = await _versionDao.FindVersionAsync(productId, clVersionValue);
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

        private async Task MakeAllLinesPendingAsync(IMakeAllChangeLogLinesPendingOutputPort output, ClVersion clVersion)
        {
            if (IsVersionReadOnly(output, clVersion))
                return;

            _unitOfWork.Start();

            var productId = clVersion.ProductId;
            var versionId = clVersion.Id;
            var pendingChangeLogs = await _changeLogQueries.GetChangeLogsAsync(productId);
            var versionChangeLogs = await _changeLogQueries.GetChangeLogsAsync(productId, versionId);

            if (versionChangeLogs.Count > pendingChangeLogs.RemainingPositionsToAdd)
            {
                output.TooManyPendingLines(ChangeLogs.MaxLines);
                return;
            }

            var duplicates = pendingChangeLogs
                .FindDuplicateTexts(versionChangeLogs.Lines)
                .ToList();

            if (duplicates.Any())
            {
                output.LineWithSameTextAlreadyExists(duplicates.Select(x => x.Text.Value).ToList());
                return;
            }

            var nextFreePosition = pendingChangeLogs.NextFreePosition;
            var lines = versionChangeLogs.Lines.Select((x, i) => new ChangeLogLine(x.Id,
                null, x.ProductId,
                x.Text, nextFreePosition + (uint) i,
                x.CreatedAt, x.Labels, x.Issues, x.CreatedByUser, x.DeletedAt));

            await MoveLinesAsync(output, productId, lines);
        }

        private static bool IsVersionReadOnly(IMakeAllChangeLogLinesPendingOutputPort output, ClVersion clVersion)
        {
            if (clVersion.IsReleased)
            {
                output.VersionAlreadyReleased(clVersion.Id);
                return true;
            }

            if (clVersion.IsDeleted)
            {
                output.VersionDeleted(clVersion.Id);
                return true;
            }

            return false;
        }

        private async Task MoveLinesAsync(IMakeAllChangeLogLinesPendingOutputPort output,
            Guid productId,
            IEnumerable<ChangeLogLine> lines)
        {
            var changeLogLines = lines.ToList();

            await _changeLogCommands.MoveLinesAsync(changeLogLines)
                .Match(Finish, output.Conflict);

            void Finish(int count)
            {
                _unitOfWork.Commit();
                output.MadePending(productId, count);
            }
        }
    }
}