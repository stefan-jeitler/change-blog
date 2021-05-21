using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Text.Json;
using ChangeTracker.Domain.ChangeLog;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class IssuesTypeHandler : SqlMapper.TypeHandler<IImmutableSet<Issue>>
    {
        public override void SetValue(IDbDataParameter parameter, IImmutableSet<Issue> value)
        {
            parameter.Value = JsonSerializer.Serialize(value.Select(x => x.Value));
        }

        public override IImmutableSet<Issue> Parse(object value)
        {
            var json = value.ToString();

            if (json is null)
                return ImmutableHashSet<Issue>.Empty;

            var issues = JsonSerializer
                .Deserialize<List<string>>(json)?
                .Select(Issue.Parse)
                .ToImmutableHashSet();

            return issues ?? ImmutableHashSet<Issue>.Empty;
        }
    }
}