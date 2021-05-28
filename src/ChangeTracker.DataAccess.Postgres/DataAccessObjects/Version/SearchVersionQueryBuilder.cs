using System;
using System.Collections.Generic;
using System.Linq;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Version
{
    public class SearchVersionQueryBuilder
    {
        private readonly List<string> _predicates = new();
        private readonly Dictionary<string, object> _parameters = new();

        public SearchVersionQueryBuilder(Guid productId)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            _parameters.Add("productId", productId);
        }

        public SearchVersionQueryBuilder AddLastVersionId(Guid? lastVersionId)
        {
            if (!lastVersionId.HasValue || lastVersionId.Value == Guid.Empty)
                return this;

            _predicates.Add("(v.created_at, v.id) < ((select vs.created_at from version vs where vs.id = @lastVersionId), @lastVersionId)");
            _parameters.Add("lastVersionId", lastVersionId.Value);

            return this;
        }

        public SearchVersionQueryBuilder ExcludeDeletedVersions()
        {
            _predicates.Add("v.deleted_at is null");
            return this;
        }

        public SearchVersionQueryBuilder AddTextSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return this;

            var trimmedSearchTerm = searchTerm.Trim();
            var finalSearchTerm = trimmedSearchTerm.Contains(" ")
                ? string.Join(" & ", trimmedSearchTerm.Split(" "))
                : trimmedSearchTerm;

            _predicates.Add("v.search_vectors @@ to_tsquery(trim(@searchTerm))");
            _parameters.Add("searchTerm", finalSearchTerm);
            return this;
        }

        public (string, IReadOnlyDictionary<string, object>) Build(ushort limit)
        {
            var predicates = _predicates.Any()
                ? $"and {string.Join(" and ", _predicates)}"
                : string.Empty;

            var query = $@"
                select v.id,
                       v.product_id      as productId,
                       v.value           as versionValue,
                       v.name,
                       v.released_at     as releasedAt,
                       v.created_by_user as createdByUser,
                       v.created_at      as createdAt,
                       v.deleted_at      as deletedAt
                from version v
                where v.product_id = @productId
                  {predicates}
                order by v.created_at desc
                fetch first (@limit) rows only";

            _parameters.Add("limit", (int)limit);

            return (query, _parameters);
        }
    }
}