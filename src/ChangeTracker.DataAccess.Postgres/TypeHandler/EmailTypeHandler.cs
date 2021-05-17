using System.Data;
using ChangeTracker.Domain.Common;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
    {
        public override void SetValue(IDbDataParameter parameter, Email value)
        {
            parameter.Value = value.Value;
        }

        public override Email Parse(object value)
        {
            return Email.Parse(value.ToString());
        }
    }
}