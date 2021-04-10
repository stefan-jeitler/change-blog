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
    public class ChangeLogDaoMock : IChangeLogDao
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

            ChangeLogs.AddRange(lines);
            return Result.Success<int, Conflict>(lines.Count);
        }

        public async Task<ChangeLogInfo> GetChangeLogInfoAsync(Guid projectId, Guid versionId)
        {
            await Task.Yield();

            var changeLogLines = ChangeLogs
                .Where(x => x.ProjectId == projectId && versionId == x.VersionId)
                .ToList();

            return new ChangeLogInfo(projectId, versionId,
                (uint) changeLogLines.Count,
                changeLogLines
                    .Select(x => (int)x.Position)
                    .DefaultIfEmpty(-1)
                    .Max());
        }

        public async Task<ChangeLogInfo> GetPendingChangeLogInfoAsync(Guid projectId)
        {
            await Task.Yield();

            var changeLogLines = ChangeLogs
                .Where(x => x.ProjectId == projectId && x.VersionId is null)
                .ToList();

            return new ChangeLogInfo(projectId, null,
                (uint) changeLogLines.Count,
                changeLogLines
                    .Select(x => (int)x.Position)
                    .DefaultIfEmpty(-1)
                    .Max());
        }
    }
}