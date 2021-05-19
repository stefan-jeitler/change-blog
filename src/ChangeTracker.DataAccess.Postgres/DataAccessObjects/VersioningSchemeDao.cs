﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Products;
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

        public async Task<Maybe<VersioningScheme>> FindSchemeAsync(Guid versioningSchemeId)
        {
            const string findVersioningSchemeSql = @"
                SELECT id,
                       name,
                       regex_pattern AS regexPattern,
                       account_id    AS accountId,
                       description,
                       created_at    AS createdAt,
                       deleted_at    AS deletedAt
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

        public async Task<VersioningScheme> GetSchemeAsync(Guid versioningSchemeId)
        {
            const string getSchemeSql = @"
                SELECT id,
                       name,
                       regex_pattern AS regexPattern,
                       account_id    AS accountId,
                       description,
                       created_at    AS createdAt,
                       deleted_at    AS deletedAt
                FROM versioning_scheme vs
                WHERE id = @schemeId";

            return await _dbAccessor.DbConnection
                .QuerySingleOrDefaultAsync<VersioningScheme>(getSchemeSql, new
                {
                    schemeId = versioningSchemeId
                });
        }

        public async Task<IList<VersioningScheme>> GetSchemesAsync(IList<Guid> versioningSchemeIds)
        {
            const string getSchemeSql = @"
                SELECT id,
                       name,
                       regex_pattern AS regexPattern,
                       account_id    AS accountId,
                       description,
                       created_at    AS createdAt,
                       deleted_at    AS deletedAt
                FROM versioning_scheme vs
                WHERE id = ANY(@schemeIds)";

            var schemes = await _dbAccessor.DbConnection
                .QueryAsync<VersioningScheme>(getSchemeSql, new
                {
                    schemeIds = versioningSchemeIds
                });

            return schemes.ToList();
        }
    }
}