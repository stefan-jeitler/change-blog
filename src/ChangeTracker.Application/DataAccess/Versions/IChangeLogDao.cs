using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IChangeLogDao
    {
        Task<Maybe<ChangeLogLine>> GetAsync(Guid changeLogLineId);
        Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine);
        Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines);
        Task<ChangeLogsMetadata> GetChangeLogsMetadataAsync(Guid projectId, Guid versionId);
        Task<ChangeLogsMetadata> GetPendingChangeLogMetadataAsync(Guid projectId);
        Task<Result<int, Conflict>> UpdateLineAsync(ChangeLogLine line);
    }
}