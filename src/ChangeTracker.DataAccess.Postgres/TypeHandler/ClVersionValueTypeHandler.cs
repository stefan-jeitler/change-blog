using System.Data;
using ChangeTracker.Domain.Version;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class ClVersionValueTypeHandler : SqlMapper.TypeHandler<ClVersionValue>
    {
        public override void SetValue(IDbDataParameter parameter, ClVersionValue value)
        {
            parameter.Value = value.Value;
        }

        public override ClVersionValue Parse(object value)
        {
            return ClVersionValue.Parse(value.ToString());
        }
    }
}