﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.ChangeLogs
{
    public interface IChangeLogCommandsDao
    {
        Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine);
        Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines);

        Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine);
        Task<Result<int, Conflict>> MoveLinesAsync(IEnumerable<ChangeLogLine> changeLogLines);
        
        Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine);
        Task<Result<ChangeLogLine, Conflict>> DeleteLineAsync(ChangeLogLine changeLogLine);
    }
}