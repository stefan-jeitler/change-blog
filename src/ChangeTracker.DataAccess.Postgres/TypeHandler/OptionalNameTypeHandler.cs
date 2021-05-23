using System.Data;
using ChangeTracker.Domain.Common;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class OptionalNameTypeHandler : SqlMapper.TypeHandler<OptionalName>
    {
        public override void SetValue(IDbDataParameter parameter, OptionalName value)
        {
            parameter.Value = value.Value;
        }

        public override OptionalName Parse(object value) => OptionalName.Parse(value.ToString());
    }
}