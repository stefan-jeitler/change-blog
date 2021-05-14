using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.DataAccess.Postgres
{
    public class SchemaVersion
    {
        private readonly Func<IDbConnection> _acquireDbConnection;
        private readonly ILogger<SchemaVersion> _logger;

        public SchemaVersion(Func<IDbConnection> acquireDbConnection, ILogger<SchemaVersion> logger)
        {
            _acquireDbConnection = acquireDbConnection;
            _logger = logger;
        }

        public static int AppSchemaVersion => 22;

        public async Task VerifySchemaVersionAsync()
        {
            using var dbConnection = _acquireDbConnection();
            const string schemaVersionSql = "SELECT version from schema_Version";
            var dbSchemaVersion = await dbConnection.ExecuteScalarAsync<int>(schemaVersionSql);

            if (AppSchemaVersion != dbSchemaVersion)
            {
                _logger.LogWarning("Schema version mismatch: App {AppSchemaVersion}; Database {dbSchemaVersion}",
                    AppSchemaVersion, dbSchemaVersion);
            }
        }
    }
}