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
                        position = (int)l.Position,
                        createdByUser = l.CreatedByUser,
                        deletedAt = l.DeletedAt,
                        createdAt = l.CreatedAt
                    });

                return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
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
                var count = await _dbAccessor.DbConnection
                    .ExecuteAsync(InsertLineSql, changeLogLines.Select(l => new
                    {
                        id = l.Id,
                        versionId = l.VersionId,
                        productId = l.ProductId,
                        text = l.Text,
                        labels = l.Labels.AsEnumerable(),
                        issues = l.Issues.AsEnumerable(),
                        position = (int)l.Position,
                        createdByUser = l.CreatedByUser,
                        deletedAt = l.DeletedAt,
                        createdAt = l.CreatedAt
                    }));

                return Result.Success<int, Conflict>(count);
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
                await _dbAccessor.DbConnection
                    .ExecuteAsync(MoveLineSql, new
                    {
                        versionId = changeLogLine.VersionId,
                        changeLogLineId = changeLogLine.Id
                    });

                return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
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
                var count = await _dbAccessor.DbConnection
                    .ExecuteAsync(MoveLineSql, changeLogLines.Select(l => new
                    {
                        versionId = l.VersionId,
                        changeLogLineId = l.Id
                    }));

                return Result.Success<int, Conflict>(count);
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
                await _dbAccessor.DbConnection
                    .ExecuteAsync(UpdateLineSql, new
                    {
                        changeLogLineId = changeLogLine.Id,
                        text = changeLogLine.Text,
                        labels = changeLogLine.Labels,
                        issues = changeLogLine.Issues
                    });

                return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
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
                await _dbAccessor.DbConnection
                    .ExecuteAsync(DeleteLineSql, new
                    {
                        changeLogLineId = changeLogLine.Id,
                        deletedAt = changeLogLine.DeletedAt
                    });

                return Result.Success<ChangeLogLine, Conflict>(changeLogLine);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while deleting a change log line");
                throw;
            }
        }
    }
}