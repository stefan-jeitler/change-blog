using System.Data;
using ChangeBlog.Domain.Miscellaneous;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.TypeHandler;

public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
{
    public override void SetValue(IDbDataParameter parameter, Email value)
    {
        parameter.Value = value.Value;
    }

    public override Email Parse(object value) => Email.Parse(value.ToString());
}