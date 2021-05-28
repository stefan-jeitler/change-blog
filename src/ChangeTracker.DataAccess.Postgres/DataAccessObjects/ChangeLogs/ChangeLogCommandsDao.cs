using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.ChangeLogs
{
    public class ChangeLogCommandsDao : IChangeLogCommandsDao
    {
        private const string InsertLineSql = @"
            insert into changelog_line (id, version_id, product_id, text, labels, issues, ""position"", created_by_user, deleted_at, created_at)
            values (@id, @versionId, @productId, @text,
                    CAST(@labels AS json), CAST(@issues AS json), @position,
                    @createdByUser, @deletedAt, @createdAt)
                ";

        private const string MoveLineSql = @"
                update changelog_line
                set version_id = @versionId
                where id = @changeLogLineId";

        private const string UpdateLineSql = @"
            update changelog_line
            set text = @text,
                labels = @labels,
                issues = @issues
            where id = @changeLogLineId";

        private const string DeleteLineSql = @"
            update changelog_line
            set deleted_at = @deletedAt
            where id = @changeLogLineId";

        private readonly IDbAccessor _dbAccessor;
        private readonly ILogger<ChangeLogCommandsDao> _logger;

        public ChangeLogCommandsDao(IDbAccessor dbAccessor, ILogger<ChangeLogCommandsDao> logger)
        {
            _dbAccessor = dbAccessor;
            _logger = logger;
        }

        public async Task<Result<ChangeLogLine, Conflict>> AddLineAsync(ChangeLogLine changeLogLine)
        {
            try
            {
                return await AddLineInternalAsync(changeLogLine);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while inserting a new change log line");
                throw;
            }
        }

        public async Task<Result<int, Conflict>> AddLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            try
            {
                return await AddLinesInternalAsync(changeLogLines.AsList());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while inserting new change log lines");
                throw;
            }
        }

        public async Task<Result<ChangeLogLine, Conflict>> MoveLineAsync(ChangeLogLine changeLogLine)
        {
            try
            {
                return await MoveLineInternalAsync(changeLogLine);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while moving a change log line");
                throw;
            }
        }

        public async Task<Result<int, Conflict>> MoveLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            try
            {
                return await MoveLinesInternalAsync(changeLogLines);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while moving a change log line");
                throw;
            }
        }

        public async Task<Result<ChangeLogLine, Conflict>> UpdateLineAsync(ChangeLogLine changeLogLine)
        {
            try
            {
                return await UpdateLineInternalAsync(changeLogLine);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while updating a change log line");
                throw;
            }
        }

        public async Task<Result<ChangeLogLine, Conflict>> DeleteLineAsync(ChangeLogLine changeLogLine)
        {
            try
            {
                return await DeleteLineInternalAsync(changeLogLine);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting a change log line");
                throw;
            }
        }

        private async Task<Result<ChangeLogLine, Conflict>> MoveLineInternalAsync(ChangeLogLine changeLogLine)
        {
            var currentVersionId = await GetVersionIdAsync(changeLogLine);
            var newVersionId = changeLogLine.VersionId;

            await _dbAccessor.DbConnection
                .ExecuteAsync(MoveLineSql, new
                {
                    versionId = changeLogLine.VersionId,
                    changeLogLineId = changeLogLine.Id
                });


            if (currentVersionId.HasValue)
                await UpdateSearchVectorsAsync(currentVersionId.Value);

            if (newVersionId.HasValue)
                await UpdateSearchVectorsAsync(newVersionId.Value);

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        private async Task<Result<int, Conflict>> MoveLinesInternalAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.AsList();

            var versionIds = await GetVersionIdsAsync(lines);

            var count = await _dbAccessor.DbConnection
                .ExecuteAsync(MoveLineSql, lines.Select(l => new
                {
                    versionId = l.VersionId,
                    changeLogLineId = l.Id
                }));

            var versionIdsOfNewDestination = await GetVersionIdsAsync(lines);

            foreach (var versionId in versionIds.Concat(versionIdsOfNewDestination))
            {
                await UpdateSearchVectorsAsync(versionId);
            }

            return Result.Success<int, Conflict>(count);
        }

        private async Task<Result<ChangeLogLine, Conflict>> AddLineInternalAsync(ChangeLogLine changeLogLine)
        {
            var l = changeLogLine;
            await _dbAccessor.DbConnection
                .ExecuteAsync(InsertLineSql, new
                {
                    id = l.Id,
                    versionId = l.VersionId,
                    prodcutId = l.ProductId,
                    text = l.Text,
                    labels = l.Labels.AsEnumerable(),
                    issues = l.Issues.AsEnumerable(),
                    position = (int) l.Position,
                    createdByUser = l.CreatedByUser,
                    deletedAt = l.DeletedAt,
                    createdAt = l.CreatedAt
                });

            if (l.VersionId.HasValue)
            {
                await UpdateSearchVectorsAsync(l.VersionId.Value);
            }

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        private async Task<Result<int, Conflict>> AddLinesInternalAsync(IReadOnlyCollection<ChangeLogLine> lines)
        {
            var count = await _dbAccessor.DbConnection
                .ExecuteAsync(InsertLineSql, lines.Select(l => new
                {
                    id = l.Id,
                    versionId = l.VersionId,
                    productId = l.ProductId,
                    text = l.Text,
                    labels = l.Labels.AsEnumerable(),
                    issues = l.Issues.AsEnumerable(),
                    position = (int) l.Position,
                    createdByUser = l.CreatedByUser,
                    deletedAt = l.DeletedAt,
                    createdAt = l.CreatedAt
                }));

            foreach (var versionId in lines
                .Where(x => x.VersionId.HasValue)
                .Select(x => x.VersionId.Value)
                .Distinct())
            {
                await UpdateSearchVectorsAsync(versionId);
            }

            return Result.Success<int, Conflict>(count);
        }

        private async Task<Result<ChangeLogLine, Conflict>> UpdateLineInternalAsync(ChangeLogLine changeLogLine)
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(UpdateLineSql, new
                {
                    changeLogLineId = changeLogLine.Id,
                    text = changeLogLine.Text,
                    labels = changeLogLine.Labels,
                    issues = changeLogLine.Issues
                });

            if (changeLogLine.VersionId.HasValue)
            {
                await UpdateSearchVectorsAsync(changeLogLine.VersionId.Value);
            }

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        private async Task<Result<ChangeLogLine, Conflict>> DeleteLineInternalAsync(ChangeLogLine changeLogLine)
        {
            await _dbAccessor.DbConnection
                .ExecuteAsync(DeleteLineSql, new
                {
                    changeLogLineId = changeLogLine.Id,
                    deletedAt = changeLogLine.DeletedAt
                });

            if (changeLogLine.VersionId.HasValue)
            {
                await UpdateSearchVectorsAsync(changeLogLine.VersionId.Value);
            }

            return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
        }

        private Task UpdateSearchVectorsAsync(Guid versionId) =>
            _dbAccessor.DbConnection
                .ExecuteAsync("CALL update_version_searchvectors_proc(@versionId)",
                    new
                    {
                        versionId
                    });

        private async Task<IList<Guid>> GetVersionIdsAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            const string sql = @"
                select distinct version_id from changelog_line
                where id = ANY (@changeLogLineIds)
                and version_id is not null";

            var versionIds = await _dbAccessor.DbConnection
                .QueryAsync<Guid>(sql, new
                {
                    changeLogLineIds = changeLogLines.Select(x => x.Id)
                });

            return versionIds.AsList();
        }

        private async Task<Guid?> GetVersionIdAsync(ChangeLogLine line)
        {
            const string sql = "select version_id from changelog_line where id = @changeLogLineId";

            return await _dbAccessor.DbConnection
                .ExecuteScalarAsync<Guid?>(sql, new
                {
                    changeLogLineId = line.Id
                });
        }
    }
}