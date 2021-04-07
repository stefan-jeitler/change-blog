using System.Data;

namespace ChangeTracker.DataAccess.Postgres
{
    public interface IDbAccessor
    {
        IDbConnection DbConnection { get; }
    }
}