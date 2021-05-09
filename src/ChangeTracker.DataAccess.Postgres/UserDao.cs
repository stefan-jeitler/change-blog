using System;
using System.Threading.Tasks;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres
{
    public class UserDao
    {
        private readonly IDbAccessor _dbAccessor;

        public UserDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<Guid?> FindUserId(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                return null;
            }

            const string getApiKeySql = @"
                SELECT u.id
                FROM api_key ak
                JOIN ""user"" u ON ak.user_id = u.id
                WHERE ak.key = @apiKey
                ";


            return await _dbAccessor.DbConnection
                .QuerySingleOrDefaultAsync<Guid?>(getApiKeySql, new {apiKey});
        }
    }
}