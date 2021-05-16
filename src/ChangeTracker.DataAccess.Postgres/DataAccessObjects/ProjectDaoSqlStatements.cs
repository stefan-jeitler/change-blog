﻿namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
{
    public static class ProjectDaoSqlStatements
    {
        public const string FindProjectByAccountAndNameSql = @"
            SELECT p.id,
                   p.account_id      AS accountId,
                   p.name,
                   vs.id             AS versioningSchemeId,
                   vs.name           AS versioningSchemeName,
                   vs.regex_pattern  AS regexPattern,
                   vs.description,
                   vs.created_at     AS versioningSchemeCreatedAt,
                   vs.deleted_at     AS versioningSchemeDeletedAt,
                   p.created_by_user AS createdByUser,
                   p.created_at      AS createdAt,
                   p.closed_at       AS closedAt
            FROM project p
                     JOIN account a on p.account_id = a.id
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE a.id = @accountId
              AND LOWER(p.name) = @name";

        public const string FindProjectByProjectIdSql = @"
            SELECT p.id,
                   p.account_id      AS accountId,
                   p.name,
                   vs.id             AS versioningSchemeId,
                   vs.name           AS versioningSchemeName,
                   vs.regex_pattern  AS regexPattern,
                   vs.description,
                   vs.created_at     AS versioningSchemeCreatedAt,
                   vs.deleted_at     AS versioningSchemeDeletedAt,
                   p.created_by_user AS createdByUser,
                   p.created_at      AS createdAt,
                   p.closed_at       AS closedAt
            FROM project p
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE p.id = @projectId";

        public static string GetProjectsForAccountSql(bool usePaging, bool includeClosedProjects)
        {
            var pagingFilter = usePaging
                ? "AND LOWER(p.name) > (Select LOWER(ps.name) from project ps where ps.id = @lastProjectId)"
                : string.Empty;

            var includeClosedProjectsFilter = includeClosedProjects 
                ? string.Empty 
                : "AND p.closed_at IS NULL";

            return @$"SELECT p.id,
                       p.account_id      AS accountId,
                       p.name,
                       vs.id             AS versioningSchemeId,
                       vs.name           AS versioningSchemeName,
                       vs.regex_pattern  AS regexPattern,
                       vs.description,
                       vs.created_at     AS versioningSchemeCreatedAt,
                       vs.deleted_at     AS versioningSchemeDeletedAt,
                       p.created_by_user AS createdByUser,
                       p.created_at      AS createdAt,
                       p.closed_at       AS closedAt
                FROM project p
                         JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
                WHERE EXISTS
                    (SELECT NULL
                     FROM account a
                              JOIN account_user au ON a.id = au.account_id and a.id = p.account_id
                              JOIN role r ON au.role_id = r.id
                              JOIN role_permission rp on r.id = rp.role_id
                     WHERE au.user_id = @userId
                       AND rp.permission = @permission
                     UNION ALL
                     SELECT NULL
                     FROM project_user pu
                              JOIN role r on pu.role_id = r.id
                              JOIN role_permission rp on r.id = rp.role_id
                     WHERE pu.user_id = @userId
                       AND rp.permission = @permission
                       AND EXISTS(
                             SELECT NULL
                             FROM account_user au1
                                      JOIN role r1 ON au1.role_id = r1.id
                             WHERE au1.user_id = @userId
                               AND r1.name = 'DefaultUser'
                               AND au1.account_id = p.account_id
                         )
                    )
                  AND p.account_id = @accountId 
                  {pagingFilter}
                  {includeClosedProjectsFilter}
                ORDER BY name
                FETCH FIRST (@count) ROWS ONLY";
        }
    }
}