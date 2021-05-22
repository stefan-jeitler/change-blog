using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Domain.ChangeLog;
using CSharpFunctionalExtensions;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.ChangeLogs
{
    public class ChangeLogQueriesDao : IChangeLogQueriesDao
    {
        private readonly IDbAccessor _dbAccessor;

        public ChangeLogQueriesDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<Maybe<ChangeLogLine>> FindLineAsync(Guid changeLogLineId)
        {
            const string findLineSql = @"
                select chl.id,
                       chl.version_id as versionId,
                       chl.product_id as productId,
                       chl.text,
                       chl.position,
                       chl.created_at as createdAt,
                       CAST(chl.labels AS text) as labels,
                       CAST(chl.issues AS text) as issues,
                       chl.deleted_at as deletedAt
                from changelog_line chl
                where id = @changeLogLineId";

            var line = await _dbAccessor.DbConnection
                .QuerySingleOrDefaultAsync<ChangeLogLine>(findLineSql, new
                {
                    changeLogLineId
                });

            return line == default
                ? Maybe<ChangeLogLine>.None
                : Maybe<ChangeLogLine>.From(line);
        }

        public async Task<Domain.ChangeLog.ChangeLogs> GetChangeLogsAsync(Guid productId, Guid? versionId = null)
        {
            var versionIdFilter = versionId.HasValue
                ? "and version_id = @versionId"
                : string.Empty;

            var getLinesSql = $@"
                select chl.id,
                       chl.version_id as versionId,
                       chl.product_id as productId,
                       chl.text,
                       chl.position,
                       chl.created_at as createdAt,
                       CAST(chl.labels AS text) as labels,
                       CAST(chl.issues AS text) as issues,
                       chl.deleted_at as deletedAt
                from changelog_line chl
                where product_id = @productId
                {versionIdFilter}";

            var lines = await _dbAccessor.DbConnection
                .QueryAsync<ChangeLogLine>(getLinesSql, new
                {
                    productId,
                    versionId
                });

            return new Domain.ChangeLog.ChangeLogs(lines.AsList());
        }

        public async Task<IList<Domain.ChangeLog.ChangeLogs>> GetChangeLogsAsync(IList<Guid> versionIds)
        {
            const string getChangeLogLinesSql = @"
                select chl.id,
                       chl.version_id           as versionId,
                       chl.product_id           as productId,
                       chl.text,
                       chl.position,
                       chl.created_at           as createdAt,
                       CAST(chl.labels AS text) as labels,
                       CAST(chl.issues AS text) as issues,
                       chl.deleted_at           as deletedAt
                from changelog_line chl
                where chl.version_id = ANY (@versionIds)";

            var lines = await _dbAccessor.DbConnection
                .QueryAsync<ChangeLogLine>(getChangeLogLinesSql, new {versionIds});

            return lines
                .GroupBy(l => l.VersionId)
                .Select(g => new Domain.ChangeLog.ChangeLogs(g.ToList()))
                .ToList();
        }

        public async Task<IList<ChangeLogLine>> GetPendingLinesAsync(Guid productId)
        {
            const string getPendingLinesSql = @"
                select chl.id,
                       chl.version_id as versionId,
                       chl.product_id as productId,
                       chl.text,
                       chl.position,
                       chl.created_at as createdAt,
                       CAST(chl.labels AS text) as labels,
                       CAST(chl.issues AS text) as issues,
                       chl.deleted_at as deletedAt
                from changelog_line chl
                where chl.product_id = @productId
                and chl.version_id is null
                ";

            var line = await _dbAccessor.DbConnection
                .QueryAsync<ChangeLogLine>(getPendingLinesSql, new
                {
                    productId
                });

            return line.AsList();
        }
    }
}