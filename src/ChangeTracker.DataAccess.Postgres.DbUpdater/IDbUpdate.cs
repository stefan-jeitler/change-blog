using System.Data;
using System.Threading.Tasks;

namespace ChangeTracker.DataAccess.Postgres.DbUpdater
{
    public interface IDbUpdate
    {
        uint NewVersion { get; }
        string ShortDescription { get; }
        Task ExecuteAsync(IDbConnection dbConnection);
    }
}