using System.Data;
using ChangeBlog.Domain.Miscellaneous;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.TypeHandler;

public class OptionalNameTypeHandler : SqlMapper.TypeHandler<OptionalName>
{
    public override void SetValue(IDbDataParameter parameter, OptionalName value)
    {
        parameter.Value = value.Value;
    }

    public override OptionalName Parse(object value)
    {
        return OptionalName.Parse(value.ToString());
    }
}