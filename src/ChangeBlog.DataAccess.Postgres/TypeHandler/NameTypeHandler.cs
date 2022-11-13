using System.Data;
using ChangeBlog.Domain.Miscellaneous;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.TypeHandler;

public class NameTypeHandler : SqlMapper.TypeHandler<Name>
{
    public override void SetValue(IDbDataParameter parameter, Name value) => parameter.Value = value.Value;

    public override Name Parse(object value) => Name.Parse(value.ToString());
}