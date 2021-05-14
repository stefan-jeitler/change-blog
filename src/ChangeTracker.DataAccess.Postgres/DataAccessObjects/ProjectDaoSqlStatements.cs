using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
{
    public static class ProjectDaoSqlStatements
    {
        public static string GetProjectsForUserSql(ushort count) => @$"
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
            WHERE (EXISTS(SELECT NULL
                         FROM account a
                                  JOIN account_user au ON a.id = au.account_id and a.id = p.account_id
                                  JOIN role r ON au.role_id = r.id
                                  JOIN role_permission rp on r.id = rp.role_id
                         WHERE au.user_id = (select id from ""user"" where lower(email) = 'stefan.jeitler@outlook.com')
                    AND rp.permission = 'ViewProjects'
                    )
                    OR EXISTS(SELECT NULL
                    FROM project_user pu
                        JOIN role r on pu.role_id = r.id
                        JOIN role_permission rp on r.id = rp.role_id
                        WHERE pu.user_id = (select id from ""user"" where lower(email) = 'stefan.jeitler@outlook.com')
                    AND rp.permission = 'ViewProjects'))
                    AND p.closed_at is null
                    ORDER BY name
              FETCH FIRST {count} ROWS ONLY";

        public static string GetProjectsForAccountSql(ushort count) => @$"
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
            WHERE (EXISTS(SELECT NULL
                         FROM account a
                                  JOIN account_user au ON a.id = au.account_id and a.id = p.account_id
                                  JOIN role r ON au.role_id = r.id
                                  JOIN role_permission rp on r.id = rp.role_id
                         WHERE au.user_id = (select id from ""user"" where lower(email) = 'stefan.jeitler@outlook.com')
                           AND rp.permission = 'ViewProjects'
                )
               OR EXISTS(SELECT NULL
                         FROM project_user pu
                                  JOIN role r on pu.role_id = r.id
                                  JOIN role_permission rp on r.id = rp.role_id
                         WHERE pu.user_id = (select id from ""user"" where lower(email) = 'stefan.jeitler@outlook.com')
                           AND rp.permission = 'ViewProjects'
                      ))
                AND p.account_id = @accountId
                AND p.closed_at is null
            ORDER BY name
                FETCH FIRST {count} ROWS ONLY";

        public const string FindProjectByAccountAndNameSql = @"
                SELECT p.id,
                       p.account_id AS accountId,
                       p.name,
                       vs.id AS versioningSchemeId,
                       vs.name AS versioningSchemeName,
                       vs.regex_pattern AS regexPattern,
                       vs.description,
                       vs.created_at AS versioningSchemeCreatedAt,
                       vs.deleted_at AS versioningSchemeDeletedAt,
                       p.created_by_user AS createdByUser,
                       p.created_at AS createdAt,
                       p.closed_at AS closedAt
                FROM project p
                JOIN account a on p.account_id = a.id
                JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
                WHERE a.id = @accountId
                AND LOWER(p.name) = @name";

        public const string FindProjectByProjectIdSql = @"
                SELECT p.id,
                       p.account_id AS accountId,
                       p.name,
                       vs.id AS versioningSchemeId,
                       vs.name AS versioningSchemeName,
                       vs.regex_pattern AS regexPattern,
                       vs.description,
                       vs.created_at AS versioningSchemeCreatedAt,
                       vs.deleted_at AS versioningSchemeDeletedAt,
                       p.created_by_user AS createdByUser,
                       p.created_at AS createdAt,
                       p.closed_at AS closedAt
                FROM project p
                JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
                WHERE p.id = @projectId";
    }
}
