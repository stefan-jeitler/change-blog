using System.Data;

namespace ChangeBlog.DataAccess.Postgres
{
    public interface IDbAccessor
    {
        IDbConnection DbConnection { get; }
    }
}
