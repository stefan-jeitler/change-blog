using System.Data;
using ChangeTracker.Domain.Common;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class TextTypeHandler : SqlMapper.TypeHandler<Text>
    {
        public override void SetValue(IDbDataParameter parameter, Text value)
        {
            parameter.Value = value.Value;
        }

        public override Text Parse(object value) => Text.Parse(value.ToString());
    }
}