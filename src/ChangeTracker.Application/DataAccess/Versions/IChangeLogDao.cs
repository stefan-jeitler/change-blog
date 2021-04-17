using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IChangeLogDao
    {
        Task<Maybe<ChangeLogLine>> FindAsync(Guid changeLogLineId);
        Task<ChangeLogsMetadata> GetChangeLogsMetadataAsync(Guid projectId, Guid? versionId = null);
        Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine);
        Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines);
        Task<Result<int, Conflict>> UpdateLineAsync(ChangeLogLine line);
    }
}