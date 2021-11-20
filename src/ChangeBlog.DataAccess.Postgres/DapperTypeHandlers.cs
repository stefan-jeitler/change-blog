using ChangeBlog.DataAccess.Postgres.TypeHandler;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres;

public static class DapperTypeHandlers
{
    public static void Add()
    {
        SqlMapper.AddTypeHandler(new NameTypeHandler());
        SqlMapper.AddTypeHandler(new OptionalNameTypeHandler());
        SqlMapper.AddTypeHandler(new TextTypeHandler());
        SqlMapper.AddTypeHandler(new EmailTypeHandler());
        SqlMapper.AddTypeHandler(new ChangeLogTextTypeHandler());
        SqlMapper.AddTypeHandler(new ClVersionValueTypeHandler());
        SqlMapper.AddTypeHandler(new LabelsTypeHandler());
        SqlMapper.AddTypeHandler(new IssuesTypeHandler());
    }
}