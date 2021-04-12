using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class ChangeLogDaoMock : IChangeLogDao
    {
        public List<ChangeLogLine> ChangeLogs { get; set; } = new();
        public bool ProduceConflict { get; set; }

        public Task<Maybe<ChangeLogLine>> GetAsync(Guid changeLogLineId)
        {
            return Task.FromResult(ChangeLogs.TryFirst(x => x.Id == changeLogLineId));
        }

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

            ChangeLogs.AddRange(lines);
            return Result.Success<int, Conflict>(lines.Count);
        }

        public async Task<ChangeLogsMetadata> GetChangeLogsMetadataAsync(Guid projectId, Guid versionId)
        {
            await Task.Yield();

            var changeLogLines = ChangeLogs
                .Where(x => x.ProjectId == projectId && versionId == x.VersionId)
                .ToList();

            return new ChangeLogsMetadata(projectId, versionId,
                (uint) changeLogLines.Count,
                changeLogLines
                    .Select(x => (int) x.Position)
                    .DefaultIfEmpty(-1)
                    .Max());
        }

        public async Task<ChangeLogsMetadata> GetPendingChangeLogMetadataAsync(Guid projectId)
        {
            await Task.Yield();

            var changeLogLines = ChangeLogs
                .Where(x => x.ProjectId == projectId && x.VersionId is null)
                .ToList();

            return new ChangeLogsMetadata(projectId, null,
                (uint) changeLogLines.Count,
                changeLogLines
                    .Select(x => (int) x.Position)
                    .DefaultIfEmpty(-1)
                    .Max());
        }

        public async Task<Result<int, Conflict>> UpdateLineAsync(ChangeLogLine line)
        {
            await Task.Yield();

            if (ProduceConflict)
                return Result.Failure<int, Conflict>(new Conflict("some conflict."));

            ChangeLogs.RemoveAll(x => x.Id == line.Id);
            ChangeLogs.Add(line);

            return Result.Success<int, Conflict>(1);
        }
    }
}