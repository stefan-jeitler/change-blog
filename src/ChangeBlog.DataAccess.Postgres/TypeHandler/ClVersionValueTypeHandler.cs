using System.Data;
using ChangeBlog.Domain.Version;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.TypeHandler
{
    public class ClVersionValueTypeHandler : SqlMapper.TypeHandler<ClVersionValue>
    {
        public override void SetValue(IDbDataParameter parameter, ClVersionValue value)
        {
            parameter.Value = value.Value;
        }

        public override ClVersionValue Parse(object value) => ClVersionValue.Parse(value.ToString());
    }
}
