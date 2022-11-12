namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products;

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
                   p.language_code    AS languageCode,
                   p.created_by_user  AS createdByUser,
                   p.created_at       AS createdAt,
                   p.freezed_at        AS freezedAt
            FROM product p
                     JOIN account a on p.account_id = a.id
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE a.id = @accountId
              AND a.deleted_at is null
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
                   p.language_code    AS languageCode,
                   p.created_by_user  AS createdByUser,
                   p.created_at       AS createdAt,
                   p.freezed_at        AS freezedAt
            FROM product p
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE p.id = @productId";

    public static string GetProductsSql(bool usePaging, string additionalFilterPredicate, string nameFilter = null)
    {
        var pagingFilter = usePaging
            ? @"AND ((p.name, p.id) > ((select ps.name from product ps where ps.id = @lastProductId and ps.freezed_at is null), @lastProductId))"
            : string.Empty;

        var nameFilterPredicate = !string.IsNullOrWhiteSpace(nameFilter)
            ? "AND p.name ilike CONCAT('%', @productName, '%')"
            : "";

        var filter = $"AND p.freezed_at IS NULL {additionalFilterPredicate} {nameFilterPredicate}";

        return GetProductsQuerySql(filter, pagingFilter);
    }

    public static string GetFreezedProductsSql(bool usePaging, string additionalFilterPredicate,
        string nameFilter = null)
    {
        var pagingFilter = usePaging
            ? @"AND (not exists(select null from product ps where ps.id = @lastProductId and ps.freezed_at is not null)
                    or (p.name, p.id) > ((select ps.name from product ps where ps.id = @lastProductId), @lastProductId))"
            : string.Empty;

        var nameFilterPredicate = !string.IsNullOrWhiteSpace(nameFilter)
            ? "AND p.name ilike CONCAT('%', @productName, '%')"
            : "";

        var filter = $"AND p.freezed_at IS NOT NULL {additionalFilterPredicate} {nameFilterPredicate}";

        return GetProductsQuerySql(filter, pagingFilter);
    }

    private static string GetProductsQuerySql(string filter, string pagingFilter) =>
        @$"SELECT p.id,
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
                   p.language_code    AS languageCode,
                   p.created_by_user  AS createdByUser,
                   p.created_at       AS createdAt,
                   p.freezed_at        AS freezedAt
            FROM product p
                     JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
            WHERE exists(select null
                         from account a
                                  join account_user au on a.id = au.account_id and a.deleted_at is null
                                  join role r on au.role_id = r.id and r.name = 'DefaultUser'
                         where au.account_id = p.account_id
                           and au.user_id = @userId)
              and (exists(select null
                          from account a
                                   join account_user au on a.id = au.account_id and a.deleted_at is null
                                   join role r on au.role_id = r.id
                                   join role_permission rp on r.id = rp.role_id and rp.permission = 'ViewProduct'
                          where au.account_id = p.account_id
                            and au.user_id = @userId
                            and not exists(select null from product_user pu where pu.product_id = p.id and pu.user_id = @userId))
                or exists(select null from product_user pu2
                                        join role r2 on pu2.role_id = r2.id
                                        join role_permission rp2 on r2.id = rp2.role_id and rp2.permission = 'ViewProduct'
                                      where pu2.product_id = p.id and pu2.user_id = @userId)
                  )
            {filter}
            {pagingFilter}
            ORDER BY p.name, p.id
            FETCH FIRST (@limit) ROWS ONLY";
}