using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;

// ReSharper disable PossibleUnintendedReferenceComparison

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Version
{
    public class VersionDao : IVersionDao
    {
        private readonly IDbAccessor _dbAccessor;
        private readonly ILogger<VersionDao> _logger;

        public VersionDao(IDbAccessor dbAccessor, ILogger<VersionDao> logger)
        {
            _dbAccessor = dbAccessor;
            _logger = logger;
        }

        public async Task<Maybe<ClVersion>> FindVersionAsync(Guid productId, ClVersionValue versionValue)
        {
            const string findVersionSql = @"
                select v.id,
                       v.product_id  as productId,
                       v.value       as versionValue,
                       v.released_at as releasedAt,
                       v.created_at  as createdAt,
                       v.deleted_at  as deletedAt
                from version v
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
            const string findVersionSql = @"
                select v.id,
                       v.product_id  as productId,
                       v.value       as versionValue,
                       v.released_at as releasedAt,
                       v.created_at  as createdAt,
                       v.deleted_at  as deletedAt
                from version v
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

        public async Task<Result<ClVersion, Conflict>> AddVersionAsync(ClVersion version)
        {
            const string insertVersionSql = @"
                insert into version (id, product_id, ""value"", released_at, deleted_at, created_at)
                values (@id, @productId, @value, @releasedAt, @deletedAt, @createdAt)";

            try
            {
                await _dbAccessor.DbConnection.ExecuteAsync(insertVersionSql, new
                {
                    id = version.Id,
                    productId = version.ProductId,
                    value = version.Value,
                    releasedAt = version.ReleasedAt,
                    deletedAt = version.DeletedAt,
                    createdAt = version.CreatedAt
                });

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
            if (!version.IsDeleted) throw new Exception("Only deleted versions can be marked as deleted.");

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
            if (!version.IsReleased) throw new Exception("Only release versions can be marked as released.");

            await _dbAccessor.DbConnection
                .ExecuteAsync("update version set released_at = @releasedAt where id = @versionId", new
                {
                    releasedAt = version.ReleasedAt,
                    versionId = version.Id
                });

            return Result.Success<ClVersion, Conflict>(version);
        }
    }
}