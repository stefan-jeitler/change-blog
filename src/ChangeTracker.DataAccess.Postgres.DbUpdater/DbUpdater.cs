using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.DataAccess.Postgres.DbUpdater.DbUpdates;
using Dapper;

// ReSharper disable InvertIf

namespace ChangeTracker.DataAccess.Postgres.DbUpdater
{
    public class DbUpdater
    {
        private static readonly IEnumerable<IDbUpdate> DbUpdates =
            new IDbUpdate[]
            {
                new CreateApiKeyTable(1)
            };

        private readonly IDbConnection _dbConnection;

        public DbUpdater(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task UpdateSchemaVersionsAsync()
        {
            await CreateSchemaTableAsync();
            var latestVersion = await GetLatestVersionAsync();

            var dbUpdates = DbUpdates
                .OrderBy(x => x.NewVersion)
                .SkipWhile(x => x.NewVersion <= latestVersion)
                .ToList();

            CheckForDuplicate(dbUpdates);

            if (!dbUpdates.Any())
            {
                Console.WriteLine("No database updates have to be executed.");
                return;
            }

            foreach (var dbUpdate in dbUpdates)
            {
                var version = dbUpdate.NewVersion;
                Console.WriteLine($"Update schema to version {version:D3}: {dbUpdate.ShortDescription}");
                await dbUpdate.ExecuteAsync(_dbConnection);
                await UpdateSchemaTableToVersionAsync(version);
            }
        }

        private async Task UpdateSchemaTableToVersionAsync(uint version)
        {
            const string updateSchemaVersionSql = "UPDATE schema_version SET version = @version, updated_at = now()";

            await _dbConnection.ExecuteAsync(updateSchemaVersionSql,
                new {version = (int) version});
        }

        private static void CheckForDuplicate(IEnumerable<IDbUpdate> dbUpdates)
        {
            var duplicates = dbUpdates
                .GroupBy(x => x.NewVersion)
                .Where(x => x.Skip(1).Any())
                .ToList();

            if (duplicates.Any())
            {
                var duplicateVersions = string.Join(" - ", duplicates.Select(x => x.Key));

                throw new Exception(
                    $"There are DbUpdates with the same schema version. see versions: {duplicateVersions}");
            }
        }

        private async Task<uint> GetLatestVersionAsync()
        {
            const string versionSql = "SELECT version FROM schema_version";
            var version = (await _dbConnection.QueryAsync<uint>(versionSql)).ToList();

            if (version.Count != 1)
            {
                throw new Exception("SchemaVersion table has more than one row.");
            }

            return version.Single();
        }

        private async Task CreateSchemaTableAsync()
        {
            if (await _dbConnection.TableExistsAsync("schemaversion"))
                return;

            const string createSchemaTableSql = @"
                CREATE TABLE schema_version
                (
	                ""version"" INT NOT NULL,
	                updated_at TIMESTAMP NOT NULL
                )";

            await _dbConnection.ExecuteAsync(createSchemaTableSql);
            await _dbConnection.ExecuteAsync("INSERT INTO schema_version VALUES (0, now())");
        }
    }
}