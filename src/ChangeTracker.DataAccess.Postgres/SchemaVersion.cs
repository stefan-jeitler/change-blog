using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres
{
    public class SchemaVersion
    {
        public const int AppSchemaVersion = 15;

        private readonly Func<IDbConnection> _acquireDbConnection;

        public SchemaVersion(Func<IDbConnection> acquireDbConnection)
        {
            _acquireDbConnection = acquireDbConnection;
        }

        public async Task VerifySchemaVersionAsync()
        {
            using var dbConnection = _acquireDbConnection();
            const string schemaVersionSql = "SELECT version from schema_Version";
            var dbSchemaVersion = await dbConnection.ExecuteScalarAsync<int>(schemaVersionSql);

            if (AppSchemaVersion != dbSchemaVersion)
            {
                throw new Exception($"Schema version mismatch: App {AppSchemaVersion}; Database {dbSchemaVersion}");
            }
        }
    }
}
