namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;

public static class UserAccessDaoSqlStatements
{
    public const string FindActiveUserIdByApiKeySql = @"
            SELECT u.id
                FROM api_key ak
                         JOIN ""user"" u
                            ON u.id = ak.user_id
                            WHERE ak.key = @apiKey
                            AND u.deleted_at IS NULL
                            AND ak.deleted_at IS NULL
                            AND ak.expires_at > now()";

    public const string FindActiveUserIdByExternalUserIdSql = @"
            SELECT u.id
            FROM external_identity ei
                     JOIN ""user"" u
                          ON u.id = ei.user_id
            WHERE ei.external_user_id = @externalUserId
              AND u.deleted_at IS NULL";

    public const string GetAccountPermissionsSql = @"
            SELECT distinct '' as type,
                            r.id,
                            r.name,
                            r.description,
                            rp.permission,
                            r.created_at AS createdAt
            from account a
            join account_user au on au.account_id = a.id
            join role r on au.role_id = r.id
            join role_permission rp on r.id = rp.role_id
            where au.account_id = @accountId
            and au.user_id = @userId
            and a.deleted_at is null";

    public static string GetPermissionsByProductIdSql => BaseQuery("@productId", "select p.account_id from product p where p.id = @productId");

    public static string GetPermissionsByVersionIdSql =>
        BaseQuery("select v.product_id from version v where v.id = @versionId", 
            "select p.account_id from version v join product p on v.product_id = p.id where v.id = @versionId");

    public static string GetPermissionsByChangeLogLineIdSql => BaseQuery("select chl.product_id from changelog_line chl where chl.id = @changeLogLineId and chl.deleted_at is null",
        "select p.account_id from changelog_line chl join product p on chl.product_id = p.id where chl.id = @changeLogLineId and chl.deleted_at is null");

    private static string BaseQuery(string selectProductId, string selectAccountId) =>
        $@"select distinct 'Product' as type,
                                r.id,
                                r.name,
                                r.description,
                                rp.permission,
                                pu.created_at as createdAt
                from product p
                         join product_user pu on p.id = pu.product_id
                         join role r on pu.role_id = r.id
                         join role_permission rp on r.id = rp.role_id
                where pu.user_id = @userId
                  and pu.product_id = ({selectProductId})
                  and exists(select null from account a where a.id = p.account_id and a.deleted_at is null)
                UNION ALL
                SELECT distinct 'Account' as type,
                                r.id,
                                r.name,
                                r.description,
                                rp.permission,
                                au.created_at as createdAt
                from account a
                         join account_user au on a.id = au.account_id
                         join role r on au.role_id = r.id
                         join role_permission rp on r.id = rp.role_id
                where a.id = ({selectAccountId})
                  and au.user_id = @userId
                  and a.deleted_at is null";
}