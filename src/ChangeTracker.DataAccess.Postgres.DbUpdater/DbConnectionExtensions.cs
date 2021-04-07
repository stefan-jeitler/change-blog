using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DbUpdater
{
    public static class DbConnectionExtensions
    {
        public static async Task<bool> TableExistsAsync(this IDbConnection dbConnection, string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentException("No table name given.", nameof(tableName));

            const string tableExistsSql = @"
                SELECT EXISTS (
                   SELECT FROM information_schema.tables 
                   WHERE  table_name   = @tableName
                   )";

            return await dbConnection
                .ExecuteScalarAsync<bool>(tableExistsSql, new {tableName = tableName.ToLower()});
        }
    }
}