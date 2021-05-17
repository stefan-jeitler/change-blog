namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
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

            return @$"
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
            WHERE EXISTS
                (select null
                 from project p
                 where p.id = p.id
                   and (
                     -- permission on project level
                         (exists(select null
                                 from project_user pu
                                 where pu.project_id = p.id
                                   and pu.user_id = @userId)
                             and exists(select null
                                        from project_user pu1
                                                 join role r on pu1.role_id = r.id and
                                                                pu1.user_id = @userId
                                                 join role_permission rp
                                                      on r.id = rp.role_id and rp.permission = @permission
                                             -- needs at least 'DefaultUser' role on account
                                        where exists(select null
                                                     from account_user aui
                                                              join role r3
                                                                   on aui.role_id = r3.id and r3.name = 'DefaultUser' and
                                                                      aui.user_id = @userId and
                                                                      aui.account_id = p.account_id))
                             )
                         -- permission on account level
                         or (
                                 not exists(select null
                                            from project_user pu
                                            where pu.project_id = p.id
                                              and pu.user_id = @userId)
                                 and exists(select null
                                            from account_user au
                                                     join role r2
                                                          on au.role_id = r2.id and
                                                             au.user_id = @userId
                                                     join role_permission rp2 on r2.id = rp2.role_id
                                            where au.account_id = p.account_id
                                              and rp2.permission = @permission)
                             )))
              AND p.account_id = @accountId
              {pagingFilter}
              {includeClosedProjectsFilter}
            ORDER BY name
                FETCH FIRST (@count) ROWS ONLY";
        }
    }
}