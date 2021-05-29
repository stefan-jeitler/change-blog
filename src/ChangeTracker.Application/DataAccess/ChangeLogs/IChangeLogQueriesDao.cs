﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.ChangeLogs
{
    public interface IChangeLogQueriesDao
    {
        Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId);
        Task<Domain.ChangeLog.ChangeLogs> GetChangeLogsAsync(Guid productId, Guid? versionId = null);
        Task<IList<Domain.ChangeLog.ChangeLogs>> GetChangeLogsAsync(IList<Guid> versionIds);
        Task<IList<ChangeLogLine>> GetPendingLinesAsync(Guid productId);
    }
}