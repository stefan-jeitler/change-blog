using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DbUpdater.DbUpdates
{
    public class CreateApiKeyTable : IDbUpdate
    {
        public CreateApiKeyTable(uint version)
        {
            NewVersion = version;
        }

        public uint NewVersion { get; }
        public string ShortDescription => "Add ApiKey Table";

        public async Task ExecuteAsync(IDbConnection dbConnection)
        {
            if (await dbConnection.TableExistsAsync("apikey"))
                return;

            await CreateApiKeyTableAsync(dbConnection);
        }

        private static async Task CreateApiKeyTableAsync(IDbConnection dbConnection)
        {
            const string createApiKeyTableSql = @"
                CREATE TABLE api_key
                (
	                id uuid DEFAULT uuid_generate_v4() NOT NULL,
	                ""key"" VARCHAR(50) NOT NULL UNIQUE,
	                notes VARCHAR(255),
	                PRIMARY KEY(id)
                )";

            await dbConnection.ExecuteAsync(createApiKeyTableSql);
        }
    }
}