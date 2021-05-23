namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Product
{
    public static class ProductDaoSqlStatements
    {
        public const string FindProductByAccountAndNameSql = @"
            SELECT p.id,
                   p.account_id       AS accountId,
                   p.name,
                   vs.id              AS vsId,
                   vs.name            AS vsName,
                   vs.regex_pattern   AS vsRegexPattern,
                   vs.description     AS vsDescription,
                   vs.account_id      AS vsAccountId,
                   vs.created_by_user AS vsCreatedByUser,
                   vs.deleted_at      AS vsDeletedAt,
                   vs.created_at      AS vsCreatedAt,
                   p.created_by_user  AS createdByUser,
                   p.created_at       AS createdAt,
                   p.closed_at        AS closedAt
            FROM product p
                     JOIN account a on p.account_id = a.id
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE a.id = @accountId
              AND LOWER(p.name) = @name";

        public const string FindProductByProductIdSql = @"
            SELECT p.id,
                   p.account_id       AS accountId,
                   p.name,
                   vs.id              AS vsId,
                   vs.name            AS vsName,
                   vs.regex_pattern   AS vsRegexPattern,
                   vs.description     AS vsDescription,
                   vs.account_id      AS vsAccountId,
                   vs.created_by_user AS vsCreatedByUser,
                   vs.deleted_at      AS vsDeletedAt,
                   vs.created_at      AS vsCreatedAt,
                   p.created_by_user  AS createdByUser,
                   p.created_at       AS createdAt,
                   p.closed_at        AS closedAt
            FROM product p
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE p.id = @productId";

        public static string GetProductsForAccountSql(bool usePaging, bool includeClosedProducts)
        {
            var pagingFilter = usePaging
                ? "AND LOWER(p.name) > (Select LOWER(ps.name) from product ps where ps.id = @lastProductId)"
                : string.Empty;

            var includeClosedProductsFilter = includeClosedProducts
                ? string.Empty
                : "AND p.closed_at IS NULL";

            const string accountFilter = "AND p.account_id = @accountId";
            
            return CreateProductsQuerySql(accountFilter, pagingFilter, includeClosedProductsFilter);
        }

        public static string GetProductsForUserSql(bool usePaging, bool includeClosedProducts)
        {
            var pagingFilter = usePaging
                ? "AND LOWER(p.name) > (Select LOWER(ps.name) from product ps where ps.id = @lastProductId)"
                : string.Empty;

            var includeClosedProductsFilter = includeClosedProducts
                ? string.Empty
                : "AND p.closed_at IS NULL";

            const string accountFilter = @"
                AND p.account_id IN (select a.id
                                   from account a
                                            join account_user au on a.id = au.account_id
                                            join role r on au.role_id = r.id
                                   where au.user_id = @userId
                                    and r.name = 'DefaultUser')";

            return CreateProductsQuerySql(accountFilter, pagingFilter, includeClosedProductsFilter);
        }

        private static string CreateProductsQuerySql(string accountFilter, string pagingFilter, string includeClosedProductsFilter) =>
            @$"
            SELECT p.id,
                   p.account_id       AS accountId,
                   p.name,
                   vs.id              AS vsId,
                   vs.name            AS vsName,
                   vs.regex_pattern   AS vsRegexPattern,
                   vs.description     AS vsDescription,
                   vs.account_id      AS vsAccountId,
                   vs.created_by_user AS vsCreatedByUser,
                   vs.deleted_at      AS vsDeletedAt,
                   vs.created_at      AS vsCreatedAt,
                   p.created_by_user  AS createdByUser,
                   p.created_at       AS createdAt,
                   p.closed_at        AS closedAt
            FROM product p
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE      -- permission on product level
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
                             )
              {accountFilter}
              {pagingFilter}
              {includeClosedProductsFilter}
            ORDER BY p.name
                FETCH FIRST (@limit) ROWS ONLY";
    }
}