using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class ChangeLogDaoStub : IChangeLogQueriesDao, IChangeLogCommandsDao
    {
        public List<ChangeLogLine> ChangeLogs { get; set; } = new();
        public bool ProduceConflict { get; set; }

        public async Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict)
            {
                return new Conflict("some conflict");
            }

            ChangeLogs.Add(changeLogLine);
            return changeLogLine;
        }

        public async Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            await Task.Yield();
            var lines = changeLogLines.ToList();

            if (ProduceConflict)
                return Result.Failure<int, Conflict>(new Conflict("something went wrong."));

            ChangeLogs.AddRange(lines);
            return Result.Success<int, Conflict>(lines.Count);
        }

        public async Task<Result<int, Conflict>> MoveLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.ToList();
            foreach (var line in lines)
            {
                var movedLine = await MoveLineAsync(line);
                if (movedLine.IsFailure)
                    return Result.Failure<int, Conflict>(movedLine.Error);
            }

            return Result.Success<int, Conflict>(lines.Count);
        }

        public async Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict)
                return Result.Failure<ChangeLogLine, Conflict>(new Conflict("some conflict"));

            ChangeLogs.RemoveAll(x => x.Id == changeLogLine.Id);
            ChangeLogs.Add(changeLogLine);

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        public async Task<Result<int, Conflict>> MakeLinesPending(Guid versionId)
        {
            await Task.Yield();

            if (ProduceConflict)
            {
                return Result.Failure<int, Conflict>(new Conflict("some conflict"));
            }

            bool MatchRequestVersion(ChangeLogLine l) => l.VersionId.HasValue && l.VersionId.Value == versionId;

            var versionChangeLogLines = ChangeLogs.Where(MatchRequestVersion);

            var pendingLines = versionChangeLogLines.Select(x => new ChangeLogLine(x.Id, null, x.ProjectId, x.Text,
                    x.Position, x.CreatedAt, x.Labels, x.Issues, x.DeletedAt))
                .ToList();

            ChangeLogs.RemoveAll(MatchRequestVersion);
            ChangeLogs.AddRange(pendingLines);

            return Result.Success<int, Conflict>(pendingLines.Count);
        }

        public async Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict)
                return Result.Failure<ChangeLogLine, Conflict>(new Conflict("some conflict."));

            ChangeLogs.RemoveAll(x => x.Id == changeLogLine.Id);
            ChangeLogs.Add(changeLogLine);

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        public Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId)
        {
            return Task.FromResult(ChangeLogs.TryFirst(x => x.Id == changeLogLineId));
        }

        public async Task<ChangeLogsMetadata> GetChangeLogsMetadataAsync(Guid projectId, Guid? versionId)
        {
            await Task.Yield();

            var changeLogLines = versionId.HasValue
                ? ChangeLogs.Where(x => x.ProjectId == projectId && x.VersionId == versionId.Value).ToList()
                : ChangeLogs.Where(x => x.ProjectId == projectId && !x.VersionId.HasValue).ToList();

            return ChangeLogsMetadata.Create(projectId, changeLogLines);
        }

        public async Task<IList<ChangeLogLine>> GetPendingLines(Guid projectId)
        {
            await Task.Yield();

            return ChangeLogs
                .Where(x => x.IsPending)
                .ToList();
        }
    }
}