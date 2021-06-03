using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.ChangeLog
{
    public interface IChangeLogQueriesDao
    {
        Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId);
        Task<ChangeLogs> GetChangeLogsAsync(Guid productId, Guid? versionId = null);
        Task<IList<ChangeLogs>> GetChangeLogsAsync(IList<Guid> versionIds);
    }
}