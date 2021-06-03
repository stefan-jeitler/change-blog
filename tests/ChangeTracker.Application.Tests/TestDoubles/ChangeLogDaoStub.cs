﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class ChangeLogDaoStub : IChangeLogQueriesDao, IChangeLogCommandsDao
    {
        public List<ChangeLogLine> ChangeLogs { get; set; } = new();
        public bool ProduceConflict { get; set; }

        public async Task<Result<ChangeLogLine, Conflict>> AddOrUpdateLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict) return new Conflict("some conflict");

            ChangeLogs.Add(changeLogLine);
            return changeLogLine;
        }

        public async Task<Result<int, Conflict>> AddOrUpdateLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            await Task.Yield();
            var lines = changeLogLines.ToList();

            ChangeLogs.RemoveAll(x => lines.Any(y => x.Id == y.Id));

            if (ProduceConflict)
                return Result.Failure<int, Conflict>(new Conflict("something went wrong."));

            ChangeLogs.AddRange(lines);
            return Result.Success<int, Conflict>(lines.Count);
        }

        public async Task<Result<int, Conflict>> MoveLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.ToList();
            foreach (var line in lines)
            {
                var movedLine = await MoveLineAsync(line);
                if (movedLine.IsFailure)
                    return Result.Failure<int, Conflict>(movedLine.Error);
            }

            return Result.Success<int, Conflict>(lines.Count);
        }

        public async Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict)
                return Result.Failure<ChangeLogLine, Conflict>(new Conflict("some conflict"));

            ChangeLogs.RemoveAll(x => x.Id == changeLogLine.Id);
            ChangeLogs.Add(changeLogLine);

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        public async Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict)
                return Result.Failure<ChangeLogLine, Conflict>(new Conflict("some conflict."));

            ChangeLogs.RemoveAll(x => x.Id == changeLogLine.Id);
            ChangeLogs.Add(changeLogLine);

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        public async Task<Result<ChangeLogLine, Conflict>> DeleteLineAsync(ChangeLogLine changeLogLine)
        {
            await Task.Yield();

            if (ProduceConflict)
                return Result.Failure<ChangeLogLine, Conflict>(new Conflict("some went badly wrong"));

            ChangeLogs.RemoveAll(x => x.Id == changeLogLine.Id);
            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        public Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId)
        {
            return Task.FromResult(ChangeLogs.TryFirst(x => x.Id == changeLogLineId));
        }

        public async Task<ChangeLogs> GetChangeLogsAsync(Guid productId, Guid? versionId)
        {
            await Task.Yield();

            var changeLogLines = versionId.HasValue
                ? ChangeLogs.Where(x => x.ProductId == productId && x.VersionId == versionId.Value).ToList()
                : ChangeLogs.Where(x => x.ProductId == productId && !x.VersionId.HasValue).ToList();

            return new ChangeLogs(changeLogLines);
        }

        public async Task<IList<ChangeLogs>> GetChangeLogsAsync(IList<Guid> versionIds)
        {
            await Task.Yield();

            return ChangeLogs
                .Where(x => versionIds.Any(y => x.Id == y))
                .GroupBy(x => x.VersionId)
                .Select(x => new ChangeLogs(x.ToList()))
                .ToList();
        }
    }
}