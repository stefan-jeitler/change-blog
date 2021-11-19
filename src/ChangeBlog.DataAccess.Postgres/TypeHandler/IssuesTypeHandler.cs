using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using ChangeBlog.Domain.ChangeLog;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.TypeHandler;

public class IssuesTypeHandler : SqlMapper.TypeHandler<IEnumerable<Issue>>
{
    public override void SetValue(IDbDataParameter parameter, IEnumerable<Issue> value)
    {
        parameter.Value = JsonSerializer.Serialize(value.Select(x => x.Value));
    }

    public override IEnumerable<Issue> Parse(object value)
    {
        var json = value.ToString();

        if (json is null)
            return Enumerable.Empty<Issue>();

        var issues = JsonSerializer
            .Deserialize<List<string>>(json)?
            .Select(Issue.Parse);

        return issues ?? Enumerable.Empty<Issue>();
    }
}