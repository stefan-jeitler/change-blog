using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLog;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.ChangeLog
{
    public class ChangeLogQueriesDao : IChangeLogQueriesDao
    {
        private const string SelectChangeLogLine = @"
            select chl.id,
                   chl.version_id as versionId,
                   chl.product_id as productId,
                   chl.text,
                   chl.position,
                   chl.created_at as createdAt,
                   CAST(chl.labels AS text) as labels,
                   CAST(chl.issues AS text) as issues,
                   chl.created_by_user as createdByUser,
                   chl.deleted_at as deletedAt
            ";

        private readonly IDbAccessor _dbAccessor;

        public ChangeLogQueriesDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId)
        {
            var findLineSql = @$"
                {SelectChangeLogLine}
                from changelog_line chl
                where chl.id = @changeLogLineId
                chl.deleted_at is null";

            var line = await _dbAccessor.DbConnection
                .QuerySingleOrDefaultAsync<ChangeLogLine>(findLineSql, new
                {
                    changeLogLineId
                });

            return line == default
                ? Maybe<ChangeLogLine>.None
                : Maybe<ChangeLogLine>.From(line);
        }

        public async Task<ChangeLogs> GetChangeLogsAsync(Guid productId, Guid? versionId = null)
        {
            var versionIdFilter = versionId.HasValue
                ? "and chl.version_id = @versionId"
                : string.Empty;

            var getLinesSql = $@"
                {SelectChangeLogLine}
                from changelog_line chl
                where chl.product_id = @productId
                and chl.deleted_at is null
                {versionIdFilter}";

            var lines = await _dbAccessor.DbConnection
                .QueryAsync<ChangeLogLine>(getLinesSql, new
                {
                    productId,
                    versionId
                });

            return new ChangeLogs(lines.AsList());
        }

        public async Task<IList<ChangeLogs>> GetChangeLogsAsync(IList<Guid> versionIds)
        {
            var getChangeLogLinesSql = @$"
                {SelectChangeLogLine}
                from changelog_line chl
                where chl.version_id = ANY (@versionIds)
                and chl.deleted_at is null";

            var lines = await _dbAccessor.DbConnection
                .QueryAsync<ChangeLogLine>(getChangeLogLinesSql, new {versionIds});

            return lines
                .GroupBy(l => l.VersionId)
                .Select(g => new ChangeLogs(g.ToList()))
                .ToList();
        }
    }
}