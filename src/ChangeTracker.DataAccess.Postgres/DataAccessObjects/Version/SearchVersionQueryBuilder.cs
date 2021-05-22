using System;
using System.Collections.Generic;
using System.Linq;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects.Version
{
    public class SearchVersionQueryBuilder
    {
        private readonly List<string> _versionPredicates = new();
        private readonly List<string> _changeLogPredicates = new();
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
            
            _versionPredicates.Add("v.created_at > (select vs.created_at from version vs where vs.id = @lastVersionId)");
            _parameters.Add("lastVersionId", lastVersionId.Value);

            return this;
        }

        public SearchVersionQueryBuilder ExcludeDeletedVersions()
        {
            _versionPredicates.Add("v.deleted_at is null");
            return this;
        }

        public SearchVersionQueryBuilder AddTextSearch(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return this;

            var trimmedSearchTerm = searchTerm.Trim();
            if (trimmedSearchTerm.Contains(" "))
                return this;
            
            _changeLogPredicates.Add("chl.search_vectors @@ to_tsquery(trim(@searchTerm))");
            _parameters.Add("searchTerm", searchTerm);
            return this;
        }

        public (string, IReadOnlyDictionary<string, object>) Build(ushort limit)
        {
            var versionPredicates = _versionPredicates.Any()
                ? $"and {string.Join(" and ", _versionPredicates)}"
                : string.Empty;

            var changeLogLinePredicates = _changeLogPredicates.Any()
                ? $"and {string.Join(" and ", _changeLogPredicates)}"
                : string.Empty;
            
            var query = $@"
                select v.id,
                       v.product_id  as productId,
                       v.value       as versionValue,
                       v.released_at as releasedAt,
                       v.created_at  as createdAt,
                       v.deleted_at  as deletedAt
                from version v
                where v.product_id = @productId
                  {versionPredicates}
                  and exists(SELECT NULL
                             FROM changelog_line chl
                             where chl.product_id = v.product_id
                               and chl.version_id = v.id
                               {changeLogLinePredicates}
                      )
                order by v.created_at desc
                    fetch first (@limit) rows only";

            _parameters.Add("limit", (int)limit);

            return (query, _parameters);
        }
    }
}