using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace ChangeTracker.Application.Decorators
{
    /// <summary>
    ///     Checks whether the target version or project is read-only
    /// </summary>
    public class ChangeLogLineReadonlyCheckDecorator : IChangeLogCommandsDao
    {
        private const string LineDeletedMessage = "The requested change log line has been deleted. ChangeLogLineId {0}";

        private const string VersionDeletedMessage = "The related version has been deleted. VersionId {0}";

        private const string VersionReleasedMessage =
            "The related version has already been released and can no longe be modified. VersionId {0}";

        private const string ProjectClosedMessage =
            "The requested project is closed and no longer be modified. ProjectId {0}";

        private readonly IChangeLogCommandsDao _changeLogCommandsComponent;
        private readonly IMemoryCache _memoryCache;
        private readonly IProjectDao _projectDao;
        private readonly IVersionDao _versionDao;


        public ChangeLogLineReadonlyCheckDecorator(IChangeLogCommandsDao changeLogCommandsComponent,
            IVersionDao versionDao,
            IMemoryCache memoryCache, IProjectDao projectDao)
        {
            _changeLogCommandsComponent = changeLogCommandsComponent;
            _versionDao = versionDao;
            _memoryCache = memoryCache;
            _projectDao = projectDao;
        }

        public Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(l => _changeLogCommandsComponent.AddLineAsync(l));
        }

        public async Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.ToList();
            foreach (var line in lines)
            {
                var result = await IsReadOnlyAsync(line);
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
                var result = await IsReadOnlyAsync(line);
                if (result.IsFailure)
                    return Result.Failure<int, Conflict>(result.Error);
            }

            return await _changeLogCommandsComponent.MoveLinesAsync(lines);
        }

        public Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(_ => _changeLogCommandsComponent.MoveLineAsync(changeLogLine));
        }

        public Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(_ => _changeLogCommandsComponent.UpdateLineAsync(changeLogLine));
        }

        public Task<Result<ChangeLogLine, Conflict>> DeleteLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(_ => _changeLogCommandsComponent.DeleteLineAsync(changeLogLine));
        }


        private async Task<Result<ChangeLogLine, Conflict>> IsReadOnlyAsync(ChangeLogLine line)
        {
            if (line.DeletedAt.HasValue)
            {
                return Result.Failure<ChangeLogLine, Conflict>(
                    new Conflict(string.Format(LineDeletedMessage, line.Id)));
            }

            if (line.IsPending)
            {
                return Result.Success<ChangeLogLine, Conflict>(line);
            }

            var version = await GetVersionAsync(line.VersionId!.Value);

            if (version.IsDeleted)
            {
                return Result.Failure<ChangeLogLine, Conflict>(
                    new Conflict(string.Format(VersionDeletedMessage, version.Id)));
            }

            if (version.IsReleased)
            {
                return Result.Failure<ChangeLogLine, Conflict>(
                    new Conflict(string.Format(VersionReleasedMessage, version.Id)));
            }

            var project = await GetProjectAsync(version.ProjectId);

            if (project.IsClosed)
                return Result.Failure<ChangeLogLine, Conflict>(
                    new Conflict(string.Format(ProjectClosedMessage, project.Id)));

            return Result.Success<ChangeLogLine, Conflict>(line);
        }

        private async Task<ClVersion> GetVersionAsync(Guid versionId)
        {
            return await _memoryCache.GetOrCreate($"ReadOnlyCheck:VersionId:{versionId}",
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                    return _versionDao.GetVersionAsync(versionId);
                });
        }

        private async Task<Project> GetProjectAsync(Guid projectId)
        {
            return await _memoryCache.GetOrCreate($"ReadOnlyCheck:ProjectId:{projectId}",
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                    return _projectDao.GetProjectAsync(projectId);
                });
        }
    }
}