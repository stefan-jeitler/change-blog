using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;

// ReSharper disable PossibleUnintendedReferenceComparison

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Versions
{
    public class VersionDao : IVersionDao
    {
        private const string SelectVersion = @"
            select v.id,
                   v.product_id      as productId,
                   v.value           as versionValue,
                   v.name,
                   v.released_at     as releasedAt,
                   v.created_by_user as createdByUser,
                   v.created_at      as createdAt,
                   v.deleted_at      as deletedAt
            from version v
            ";

        private readonly IDbAccessor _dbAccessor;
        private readonly ILogger<VersionDao> _logger;

        public VersionDao(IDbAccessor dbAccessor, ILogger<VersionDao> logger)
        {
            _dbAccessor = dbAccessor;
            _logger = logger;
        }

        public async Task<Maybe<ClVersion>> FindVersionAsync(Guid productId, ClVersionValue versionValue)
        {
            var findVersionSql = $@"
                {SelectVersion}
                where v.product_id = @productId
                and v.value = @versionValue";

            var version = await _dbAccessor.DbConnection
                .QuerySingleOrDefaultAsync<ClVersion>(findVersionSql, new
                {
                    productId,
                    versionValue
                });

            return version == default
                ? Maybe<ClVersion>.None
                : Maybe<ClVersion>.From(version);
        }

        public async Task<Maybe<ClVersion>> FindVersionAsync(Guid versionId)
        {
            var findVersionSql = @$"
                {SelectVersion}
                where v.id = @versionId";

            var version = await _dbAccessor.DbConnection
                .QuerySingleOrDefaultAsync<ClVersion>(findVersionSql, new
                {
                    versionId
                });

            return version == default
                ? Maybe<ClVersion>.None
                : Maybe<ClVersion>.From(version);
        }

        public async Task<ClVersion> GetVersionAsync(Guid versionId)
        {
            var version = await FindVersionAsync(versionId);

            if (version.HasNoValue)
                throw new Exception(
                    "The requested version does not exist. If you are not sure whether the version exists use 'FindVersion' otherwise file an issue.");

            return version.Value;
        }

        public async Task<IList<ClVersion>> GetVersionsAsync(VersionQuerySettings querySettings)
        {
            var queryBuilder = new SearchVersionQueryBuilder(querySettings.ProductId)
                .AddLastVersionId(querySettings.LastVersionId)
                .AddTextSearch(querySettings.SearchTerm);

            if (!querySettings.IncludeDeleted)
                queryBuilder.ExcludeDeletedVersions();

            var (query, parameters) = queryBuilder.Build(querySettings.Limit);

            var versions = await _dbAccessor.DbConnection
                .QueryAsync<ClVersion>(query, parameters);

            return versions.AsList();
        }

        public async Task<Result<ClVersion, Conflict>> AddVersionAsync(ClVersion version)
        {
            const string insertVersionSql = @"
                insert into version (id, product_id, ""value"", name, released_at, created_by_user, deleted_at, created_at)
                values (@id, @productId, @value, @name, @releasedAt, @createdByUser, @deletedAt, @createdAt)
            ";

            try
            {
                await _dbAccessor.DbConnection.ExecuteAsync(insertVersionSql, new
                {
                    id = version.Id,
                    productId = version.ProductId,
                    value = version.Value,
                    name = version.Name.Value,
                    releasedAt = version.ReleasedAt,
                    createdByUser = version.CreatedByUser,
                    deletedAt = version.DeletedAt,
                    createdAt = version.CreatedAt
                });

                await UpdateSearchVectorsAsync(version.Id);

                return Result.Success<ClVersion, Conflict>(version);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while inserting a new version");
                throw;
            }
        }

        public async Task<Result<ClVersion, Conflict>> DeleteVersionAsync(ClVersion version)
        {
            if (!version.IsDeleted)
                throw new Exception("Only deleted versions can be marked as deleted.");

            await _dbAccessor.DbConnection
                .ExecuteAsync("update version set deleted_at = @deletedAt where id = @versionId", new
                {
                    deletedAt = version.DeletedAt,
                    versionId = version.Id
                });

            return Result.Success<ClVersion, Conflict>(version);
        }

        public async Task<Result<ClVersion, Conflict>> ReleaseVersionAsync(ClVersion version)
        {
            if (!version.IsReleased) 
                throw new Exception("Only release versions can be marked as released.");

            await _dbAccessor.DbConnection
                .ExecuteAsync("update version set released_at = @releasedAt where id = @versionId", new
                {
                    releasedAt = version.ReleasedAt,
                    versionId = version.Id
                });

            return Result.Success<ClVersion, Conflict>(version);
        }

        public async Task<Result<ClVersion, Conflict>> UpdateVersionAsync(ClVersion version)
        {
            try
            {
                await _dbAccessor.DbConnection
                    .ExecuteAsync(
                        "update version set name = @name, value = @value, released_at = @releasedAt where id = @versionId",
                        new
                        {
                            name = version.Name.Value,
                            value = version.Value.Value,
                            releasedAt = version.ReleasedAt,
                            versionId = version.Id
                        });

                await UpdateSearchVectorsAsync(version.Id);

                return Result.Success<ClVersion, Conflict>(version);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, exception.Message);
                throw;
            }
        }

        private Task UpdateSearchVectorsAsync(Guid versionId) =>
            _dbAccessor.DbConnection
                .ExecuteAsync("CALL update_version_searchvectors_proc(@versionId)",
                    new
                    {
                        versionId
                    });
    }
}