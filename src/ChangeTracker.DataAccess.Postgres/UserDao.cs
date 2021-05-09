using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres
{
    public class UserDao
    {
        private readonly Func<IDbConnection> _acquireDbConnection;

        public UserDao(Func<IDbConnection> acquireDbConnection)
        {
            _acquireDbConnection = acquireDbConnection;
        }

        public async Task<Guid?> FindUserId(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return null;
            }

            const string getApiKeySql = @"
                SELECT user_id
                FROM api_key
                WHERE key = @apiKey";

            using var dbConnection = _acquireDbConnection();
            return await dbConnection
                .QuerySingleOrDefaultAsync<Guid?>(getApiKeySql, new {apiKey});
        }
    }
}