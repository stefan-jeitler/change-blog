using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.ChangeLogs
{
    public interface IChangeLogQueriesDao
    {
        Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId);
        Task<Domain.ChangeLog.ChangeLogs> GetChangeLogsAsync(Guid projectId, Guid? versionId = null);
        Task<IList<ChangeLogLine>> GetPendingLinesAsync(Guid projectId);
    }
}