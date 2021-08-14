using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using ChangeBlog.Domain.ChangeLog;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.TypeHandler
{
    public class LabelsTypeHandler : SqlMapper.TypeHandler<IEnumerable<Label>>
    {
        public override void SetValue(IDbDataParameter parameter, IEnumerable<Label> value)
        {
            parameter.Value = JsonSerializer.Serialize(value.Select(x => x.Value));
        }

        public override IEnumerable<Label> Parse(object value)
        {
            var json = value.ToString();

            if (json is null)
                return Enumerable.Empty<Label>();

            var labels = JsonSerializer
                .Deserialize<List<string>>(json)?
                .Select(Label.Parse);

            return labels ?? Enumerable.Empty<Label>();
        }
    }
}
