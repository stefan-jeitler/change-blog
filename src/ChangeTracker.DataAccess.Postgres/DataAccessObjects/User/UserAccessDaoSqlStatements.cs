namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.User
{
    public static class UserAccessDaoSqlStatements
    {
        public const string FindUserIdByApiKeySql = @"
            SELECT u.id
                FROM api_key ak
                         JOIN ""user"" u
                            ON u.id = ak.user_id
                            WHERE ak.key = @apiKey
                            AND u.deleted_at IS NULL
                                AND ak.deleted_at IS NULL
                                AND ak.expires_at > now()";

        public const string AccountPermissionSql = @"
            SELECT EXISTS(SELECT NULL
                          FROM account a
                                   JOIN account_user au ON au.account_id = a.id
                                   JOIN ""role"" r ON r.id = au.role_id
                          JOIN role_permission rp ON rp.role_id = r.id
                          WHERE au.account_id = @accountId
                              AND au.user_id = @userId
                              AND rp.permission = @permission)";

        public const string AccountUserPermission = @"
            SELECT EXISTS(SELECT null from ""user"" u
                          join account_user au on u.id = au.user_id
                          join role r on au.role_id = r.id
                          join role_permission rp on r.id = rp.role_id
                          where u.id = @userId
                              and rp.permission = @permission)";

        public static string ProductPermissionsSql => BaseQuery("@productId");

        public static string VersionPermissionSql =>
            BaseQuery("(select p.id from version v join product p on v.product_id = p.id where v.id = @versionId)");

        public static string ChangeLogLinePermissionSql => BaseQuery(
            "(select p.id from changelog_line chl join product p on chl.product_id = p.id where chl.id = @changeLogLineId)");

        private static string BaseQuery(string selectProductId)
        {
            return $@"
                select exists(select null
                              from product p
                              where p.id = {selectProductId}
                                and (
                                  -- permission on product level overrides account permissions
                                      (exists(select null
                                              from product_user pu
                                              where pu.product_id = p.id
                                                and pu.user_id = @userId)
                                          and exists(select null
                                                     from product_user pu1
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
                                                         from product_user pu
                                                         where pu.product_id = p.id
                                                           and pu.user_id = @userId)
                                              and exists(select null
                                                         from account_user au
                                                                  join role r2
                                                                       on au.role_id = r2.id and
                                                                          au.user_id = @userId
                                                                  join role_permission rp2 on r2.id = rp2.role_id
                                                         where au.account_id = p.account_id
                                                           and rp2.permission = @permission)
                                          )))";
        }
    }
}