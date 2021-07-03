using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Application.DataAccess.Conflicts;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Caching.Memory;

// ReSharper disable ConvertIfStatementToReturnStatement

namespace ChangeTracker.Application.Proxies
{
    /// <summary>
    ///     Checks whether the target version or product is read-only
    /// </summary>
    public class ChangeLogLineReadonlyCheckProxy : IChangeLogCommandsDao
    {
        private readonly IChangeLogCommandsDao _changeLogCommands;
        private readonly IMemoryCache _memoryCache;
        private readonly IProductDao _productDao;
        private readonly IVersionDao _versionDao;

        public ChangeLogLineReadonlyCheckProxy(IChangeLogCommandsDao changeLogCommands,
            IVersionDao versionDao,
            IMemoryCache memoryCache, IProductDao productDao)
        {
            _changeLogCommands = changeLogCommands;
            _versionDao = versionDao;
            _memoryCache = memoryCache;
            _productDao = productDao;
        }

        public Task<Result<ChangeLogLine, Conflict>> AddOrUpdateLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(l => _changeLogCommands.AddOrUpdateLineAsync(l));
        }

        public async Task<Result<int, Conflict>> AddOrUpdateLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.ToList();
            foreach (var line in lines)
            {
                var result = await IsReadOnlyAsync(line);
                if (result.IsFailure)
                    return Result.Failure<int, Conflict>(result.Error);
            }

            return await _changeLogCommands.AddOrUpdateLinesAsync(lines);
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

            return await _changeLogCommands.MoveLinesAsync(lines);
        }

        public Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(_ => _changeLogCommands.MoveLineAsync(changeLogLine));
        }

        public Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(_ => _changeLogCommands.UpdateLineAsync(changeLogLine));
        }

        public Task<Result<ChangeLogLine, Conflict>> DeleteLineAsync(ChangeLogLine changeLogLine)
        {
            return IsReadOnlyAsync(changeLogLine)
                .Bind(_ => _changeLogCommands.DeleteLineAsync(changeLogLine));
        }

        public Task DeletePendingChangeLogs(Guid productId) =>
            _changeLogCommands.DeletePendingChangeLogs(productId);


        private async Task<Result<ChangeLogLine, Conflict>> IsReadOnlyAsync(ChangeLogLine line)
        {
            if (line.DeletedAt.HasValue)
                return Result.Failure<ChangeLogLine, Conflict>(
                    new ChangeLogLineDeletedConflict(line.Id));

            if (line.IsPending) return Result.Success<ChangeLogLine, Conflict>(line);

            var version = await GetVersionAsync(line.VersionId!.Value);

            if (version.IsDeleted)
                return Result.Failure<ChangeLogLine, Conflict>(
                    new VersionDeletedConflict(version.Id));

            if (version.IsReleased)
                return Result.Failure<ChangeLogLine, Conflict>(
                    new VersionReleasedConflict(version.Id));

            var product = await GetProductAsync(version.ProductId);

            if (product.IsClosed)
                return Result.Failure<ChangeLogLine, Conflict>(
                    new ProductClosedConflict(product.Id));

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

        private async Task<Product> GetProductAsync(Guid productId)
        {
            return await _memoryCache.GetOrCreate($"ReadOnlyCheck:ProductId:{productId}",
                entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
                    return _productDao.GetProductAsync(productId);
                });
        }
    }
}