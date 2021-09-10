using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.ChangeLog;
using ChangeBlog.Application.DataAccess.Conflicts;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.ChangeLog
{
    public class ChangeLogCommandsDao : IChangeLogCommandsDao
    {
        private const string InsertOrUpdateSql = @"
            insert into changelog_line (id, version_id, product_id, text, labels, issues, ""position"", created_by_user, deleted_at, created_at)
            values (@id, @versionId, @productId, @text,
                    CAST(@labels AS json), CAST(@issues AS json), @position,
                    @createdByUser, @deletedAt, @createdAt)
            on conflict (product_id, coalesce(version_id, '00000000-0000-0000-0000-000000000000'), lower(text), ((deleted_at IS NULL))) WHERE (deleted_at IS NULL) do update
                set text     = @text,
                    labels   = CAST(@labels AS json),
                    issues   = CAST(@issues AS json),
                    position = @position";

        private const string MoveLineSql = @"
            update changelog_line
            set version_id = @versionId, position = @position
            where id = @changeLogLineId";

        private const string UpdateLineSql = @"
            update changelog_line
            set text = @text,
                labels = CAST(@labels AS json),
                issues = CAST(@issues AS json)
            where id = @changeLogLineId";

        private const string DeleteLineSql = @"
            update changelog_line
            set deleted_at = now()
            where id = @changeLogLineId";

        private readonly IDbAccessor _dbAccessor;
        private readonly ILogger<ChangeLogCommandsDao> _logger;

        public ChangeLogCommandsDao(IDbAccessor dbAccessor, ILogger<ChangeLogCommandsDao> logger)
        {
            _dbAccessor = dbAccessor;
            _logger = logger;
        }

        public async Task<Result<ChangeLogLine, Conflict>> AddOrUpdateLineAsync(ChangeLogLine changeLogLine)
        {
            try
            {
                await AddOrUpdateLinesInternalAsync(new[] {changeLogLine});
                return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
            }
            //duplicate key value violates unique constraint
            catch (PostgresException postgresException) when (postgresException.SqlState == "23505")
            {
                return Result.Failure<ChangeLogLine, Conflict>(
                    new AddOrUpdateChangeLogLineConcurrencyConflict(changeLogLine.ProductId,
                        changeLogLine.VersionId,
                        changeLogLine.Id));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while inserting a new change log line");
                throw;
            }
        }

        public async Task<Result<int, Conflict>> AddOrUpdateLinesAsync(IEnumerable<ChangeLogLine> changeLogLines)
        {
            var lines = changeLogLines.AsList();

            try
            {
                return await AddOrUpdateLinesInternalAsync(lines);
            }
            //duplicate key value violates unique constraint
            catch (PostgresException postgresException) when (postgresException.SqlState == "23505")
            {
                var l = lines.First();

                return Result.Failure<int, Conflict>(
                    new AddOrUpdateChangeLogLineConcurrencyConflict(l.ProductId, l.VersionId, l.Id));
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

        public Task DeletePendingChangeLogs(Guid productId)
        {
            const string deletePendingChangeLogsSql =
                "update changelog_line set deleted_at = now() where product_id = @productId and version_id is null";

            return _dbAccessor.DbConnection.ExecuteAsync(deletePendingChangeLogsSql, new
            {
                productId
            });
        }

        private async Task<Result<ChangeLogLine, Conflict>> MoveLineInternalAsync(ChangeLogLine changeLogLine)
        {
            var currentVersionId = await GetVersionIdAsync(changeLogLine);
            var newVersionId = changeLogLine.VersionId;

            await _dbAccessor.DbConnection
                .ExecuteAsync(MoveLineSql, new
                {
                    versionId = changeLogLine.VersionId,
                    position = (int) changeLogLine.Position,
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
            var versionIdsBeforeMove = await GetVersionIdsAsync(lines.Select(x => x.Id));

            var count = await _dbAccessor.DbConnection
                .ExecuteAsync(MoveLineSql, lines.Select(l => new
                {
                    versionId = l.VersionId,
                    position = (int) l.Position,
                    changeLogLineId = l.Id
                }));

            var versionIdsOfNewDestination = lines
                .Where(x => x.VersionId.HasValue)
                .Select(x => x.VersionId.Value)
                .Distinct();

            foreach (var versionId in versionIdsBeforeMove.Concat(versionIdsOfNewDestination))
            {
                await UpdateSearchVectorsAsync(versionId);
            }

            return Result.Success<int, Conflict>(count);
        }

        private async Task<Result<int, Conflict>> AddOrUpdateLinesInternalAsync(
            IReadOnlyCollection<ChangeLogLine> lines)
        {
            var count = await _dbAccessor.DbConnection
                .ExecuteAsync(InsertOrUpdateSql, lines.Select(l => new
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
                    labels = changeLogLine.Labels.AsEnumerable(),
                    issues = changeLogLine.Issues.AsEnumerable()
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
                    changeLogLineId = changeLogLine.Id
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

        private async Task<IList<Guid>> GetVersionIdsAsync(IEnumerable<Guid> changeLogLineIds)
        {
            const string sql = @"
                select distinct version_id from changelog_line
                where id = ANY (@changeLogLineIds)
                and version_id is not null";

            var versionIds = await _dbAccessor.DbConnection
                .QueryAsync<Guid>(sql, new
                {
                    changeLogLineIds = changeLogLineIds.ToList()
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