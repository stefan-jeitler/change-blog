using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
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
                : ChangeLogs.Where(x => x.ProjectId == projectId).ToList();

            return new ChangeLogsMetadata(projectId, versionId,
                (uint) changeLogLines.Count,
                changeLogLines
                    .Select(x => (int) x.Position)
                    .DefaultIfEmpty(-1)
                    .Max());
        }
    }
}