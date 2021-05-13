using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
{
    public class VersioningSchemeDao : IVersioningSchemeDao
    {
        private readonly IDbAccessor _dbAccessor;

        public VersioningSchemeDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<Maybe<VersioningScheme>> FindAsync(Guid versioningSchemeId)
        {
            const string findVersioningSchemeSql = @"
                SELECT id,
                       name,
                       regex_pattern AS regexPattern,
                       account_id AS accountId,
                       description,
                       created_at AS createdAt,
                       deleted_at AS deletedAt
                FROM versioning_scheme
                WHERE id = @versioningSchemeId";

            var versioningScheme = await _dbAccessor.DbConnection
                .QueryFirstOrDefaultAsync<VersioningScheme>(findVersioningSchemeSql, new
                {
                    versioningSchemeId
                });

            return versioningScheme == default
                ? Maybe<VersioningScheme>.None
                : Maybe<VersioningScheme>.From(versioningScheme);
        }
    }
}