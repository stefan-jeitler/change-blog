using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Domain.ChangeLog;
using OneOf;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class ChangeLogDaoMock : IChangeLogDao
    {
        public List<ChangeLogLine> ChangeLog { get; set; } = new();
        public bool ProduceConflict { get; set; }

        public async Task<OneOf<ChangeLogLine, Conflict>> AddChangeLogLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict)
            {
                return new Conflict("some conflict");
            }

            ChangeLog.Add(changeLogLine);
            return changeLogLine;
        }

        public async Task<ChangeLogInfo> GetChangeLogInfoAsync(Guid projectId, Guid? versionId)
        {
            await Task.Yield();

            var changeLogLines = ChangeLog.Where(x =>
                    x.ProjectId == projectId && versionId.HasValue && versionId.Value == x.VersionId)
                .ToList();

            return new ChangeLogInfo(projectId, versionId,
                (uint) changeLogLines.Count,
                changeLogLines.Max(x => x.Position));
        }
    }
}