using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IChangeLogDao
    {
        Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine);
        Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines);
        Task<ChangeLogInfo> GetChangeLogInfoAsync(Guid projectId, Guid versionId);
        Task<ChangeLogInfo> GetPendingChangeLogInfoAsync(Guid projectId);
    }
}