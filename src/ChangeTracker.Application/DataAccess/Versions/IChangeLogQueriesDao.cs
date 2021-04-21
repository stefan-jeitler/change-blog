using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IChangeLogQueriesDao
    {
        Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId);
        Task<ChangeLogsMetadata> GetChangeLogsMetadataAsync(Guid projectId, Guid? versionId = null);
        Task<IList<ChangeLogLine>> GetPendingLines(Guid projectId);
    }
}