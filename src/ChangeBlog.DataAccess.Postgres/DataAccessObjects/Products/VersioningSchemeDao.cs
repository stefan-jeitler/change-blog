using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products;

public class VersioningSchemeDao : IVersioningSchemeDao
{
    private const string SelectVersioningScheme = @"
                SELECT id,
                       name,
                       regex_pattern   AS regexPattern,
                       description,
                       account_id      AS accountId,
                       created_by_user AS createdByUser,
                       deleted_at      AS deletedAt,
                       created_at      AS createdAt
                FROM versioning_scheme
                ";

    private readonly IDbAccessor _dbAccessor;

    public VersioningSchemeDao(IDbAccessor dbAccessor)
    {
        _dbAccessor = dbAccessor;
    }

    public async Task<Maybe<VersioningScheme>> FindSchemeAsync(Guid versioningSchemeId)
    {
        const string findVersioningSchemeSql = @$"
                {SelectVersioningScheme}
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

    public async Task<VersioningScheme> GetSchemeAsync(Guid versioningSchemeId)
    {
        const string getSchemeSql = @$"
                {SelectVersioningScheme}
                WHERE id = @schemeId";

        return await _dbAccessor.DbConnection
            .QuerySingleOrDefaultAsync<VersioningScheme>(getSchemeSql, new
            {
                schemeId = versioningSchemeId
            });
    }

    public async Task<IList<VersioningScheme>> GetSchemesAsync(IList<Guid> versioningSchemeIds)
    {
        const string getSchemeSql = $@"
                {SelectVersioningScheme}
                WHERE id = ANY(@schemeIds)";

        var schemes = await _dbAccessor.DbConnection
            .QueryAsync<VersioningScheme>(getSchemeSql, new
            {
                schemeIds = versioningSchemeIds
            });

        return schemes.AsList();
    }
}