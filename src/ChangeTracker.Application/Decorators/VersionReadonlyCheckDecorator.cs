using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace ChangeTracker.Application.Decorators
{
    /// <summary>
    ///     Checks whether the target version is read-only
    /// </summary>
    public class VersionReadonlyCheckDecorator : IChangeLogCommandsDao
    {
        private const string VersionDeletedMessage = "The related version has been closed. ChangeLogLineId {0}";

        private const string VersionReleasedMessage =
            "The related version has already been released. ChangeLogLineId {0}";

        private readonly IChangeLogCommandsDao _changeLogCommandsComponent;
        private readonly IMemoryCache _memoryCache;
        private readonly IVersionDao _versionDao;


        public VersionReadonlyCheckDecorator(IChangeLogCommandsDao changeLogCommandsComponent, IVersionDao versionDao,
            IMemoryCache memoryCache)
        {
            _changeLogCommandsComponent = changeLogCommandsComponent;
            _versionDao = versionDao;
            _memoryCache = memoryCache;
        }

        public Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine)
        {
            return CheckVersionAsync(changeLogLine)
                .Bind(l => _changeLogCommandsComponent.AddLineAsync(l));
        }

        public async Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.ToList();
            foreach (var line in lines)
            {
                var result = await CheckVersionAsync(line);
                if (result.IsFailure)
                    return Result.Failure<int, Conflict>(result.Error);
            }

            return await _changeLogCommandsComponent.AddLinesAsync(lines);
        }

        public async Task<Result<int, Conflict>> MoveLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.ToList();
            foreach (var line in lines)
            {
                var result = await CheckVersionAsync(line);
                if (result.IsFailure)
                    return Result.Failure<int, Conflict>(result.Error);
            }

            return await _changeLogCommandsComponent.MoveLinesAsync(lines);
        }

        public Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine)
        {
            return CheckVersionAsync(changeLogLine)
                .Bind(_ => _changeLogCommandsComponent.MoveLineAsync(changeLogLine));
        }

        public Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine)
        {
            return CheckVersionAsync(changeLogLine)
                .Bind(_ => _changeLogCommandsComponent.UpdateLineAsync(changeLogLine));
        }


        private async Task<Result<ChangeLogLine, Conflict>> CheckVersionAsync(ChangeLogLine line)
        {
            if (line.IsPending)
            {
                return Result.Success<ChangeLogLine, Conflict>(line);
            }

            var version = await GetVersionAsync(line);
            if (version.IsDeleted)
            {
                return Result.Failure<ChangeLogLine, Conflict>(
                    new Conflict(string.Format(VersionDeletedMessage, line.Id)));
            }

            if (version.IsReleased)
            {
                return Result.Failure<ChangeLogLine, Conflict>(
                    new Conflict(string.Format(VersionReleasedMessage, line.Id)));
            }

            return Result.Success<ChangeLogLine, Conflict>(line);
        }

        private async Task<ClVersion> GetVersionAsync(ChangeLogLine line)
        {
            return await _memoryCache.GetOrCreate($"VersionReadOnlyCheck:VersionId:{line.VersionId!.Value}",
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(3);
                    return _versionDao.GetVersionAsync(line.VersionId!.Value);
                });
        }
    }
}