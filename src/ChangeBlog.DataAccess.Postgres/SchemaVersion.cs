using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Semver;

namespace ChangeBlog.DataAccess.Postgres
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

        private static SemVersion AppSchemaVersion => SemVersion.Parse("12.1.4");

        public async Task ApproveAsync()
        {
            using var dbConnection = _acquireDbConnection();
            const string schemaVersionSql = "SELECT version from schema_Version";
            var dbSchemaVersionValue = await dbConnection.ExecuteScalarAsync<string>(schemaVersionSql);
            var dbSchemaVersion = SemVersion.Parse(dbSchemaVersionValue);

            if (AppSchemaVersion.Major != dbSchemaVersion.Major)
            {
                throw new Exception($"Schema version mismatch: App {AppSchemaVersion}; Database {dbSchemaVersion}");
            }

            if (AppSchemaVersion != dbSchemaVersion)
            {
                _logger.LogWarning("Schema version mismatch: App {AppSchemaVersion}; Database {dbSchemaVersion}",
                    AppSchemaVersion, dbSchemaVersion);
            }
        }
    }
}
