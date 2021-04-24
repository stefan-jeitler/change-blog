using System;
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

        /// <summary>
        /// Move lines to a version or make lines pending if the version is null.
        /// </summary>
        /// <param name="changeLogLines"></param>
        /// <returns></returns>
        Task<Result<int, Conflict>> MoveLinesAsync(IEnumerable<ChangeLogLine> changeLogLines);

        /// <summary>
        /// Moves the line to a version or make it pending if the version is null.
        /// </summary>
        /// <param name="changeLogLine"></param>
        /// <returns></returns>
        Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine);

        Task<Result<int, Conflict>> MakeLinesPending(Guid versionId);

        Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine);
    }
}