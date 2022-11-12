using System;
using System.Collections.Generic;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products;

public enum ProductType
{
    Active,
    Freezed
}

public class ProductsQueryBuilder
{
    private readonly Dictionary<string, object> _parameters;
    private readonly HashSet<string> _predicates;
    private readonly ProductType _productType;

    public ProductsQueryBuilder(Guid userId, ProductType productType)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("userId must not be empty");

        _parameters = new Dictionary<string, object>
        {
            ["userId"] = userId
        };

        var productTypePredicate = productType switch
        {
            ProductType.Active => "p.freezed_at IS NULL",
            ProductType.Freezed => "p.freezed_at IS NOT NULL",
            _ => throw new ArgumentOutOfRangeException(nameof(productType), productType, null)
        };

        _predicates = new HashSet<string> {productTypePredicate};
        _productType = productType;
    }

    public ProductsQueryBuilder TryAddPagingFilter(Guid? lastProductId)
    {
        if (!lastProductId.HasValue)
            return this;

        var pagingFilter = _productType switch
        {
            ProductType.Active =>
                @"((p.name, p.id) > ((select ps.name from product ps where ps.id = @lastProductId and ps.freezed_at is null), @lastProductId))",
            ProductType.Freezed =>
                @"(not exists(select null from product ps where ps.id = @lastProductId and ps.freezed_at is not null)
                    or (p.name, p.id) > ((select ps.name from product ps where ps.id = @lastProductId), @lastProductId))",
            _ => throw new ArgumentOutOfRangeException()
        };

        _predicates.Add(pagingFilter);
        _parameters["lastProductId"] = lastProductId.Value;

        return this;
    }

    public ProductsQueryBuilder TryAddNameFilter(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return this;

        _predicates.Add("p.name ilike CONCAT('%', @productName, '%')");
        _parameters["productName"] = name;

        return this;
    }

    public ProductsQueryBuilder AddAccountFilter(Guid accountId)
    {
        if (accountId == Guid.Empty)
            return this;

        _predicates.Add("p.account_id = @accountId");
        _parameters["accountId"] = accountId;

        return this;
    }

    public (string query, IReadOnlyDictionary<string, object> parameters) Build(ushort limit = 100)
    {
        _parameters.Add("limit", (int) limit);
        var predicates = string.Join(" AND ", _predicates);

        var query = @$"SELECT p.id,
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
            AND {predicates}
            ORDER BY p.name, p.id
            FETCH FIRST (@limit) ROWS ONLY";

        return (query, _parameters);
    }
}