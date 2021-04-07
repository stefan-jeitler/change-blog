using System;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace ChangeTracker.DataAccess.Postgres.DbUpdater
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .AddUserSecrets<Program>()
                .Build();

            var version = GetAssemblyInformationalVersion();
            Console.WriteLine($"DbUpdater version: {version}");

            var connectionString = configuration.GetConnectionString("ChangeTrackerDb");
            await using var dbConnection = new NpgsqlConnection(connectionString);

            await RunDbUpdaterAsync(dbConnection);
        }

        private static async Task RunDbUpdaterAsync(IDbConnection dbConnection)
        {
            var dbUpdater = new DbUpdater(dbConnection);
            Console.WriteLine("Start Schema Updater...");
            await dbUpdater.UpdateSchemaVersionsAsync();
            Console.WriteLine("Finished!");
        }

        private static string GetAssemblyInformationalVersion()
        {
            var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
            var assemblyVersionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
            return assemblyVersionAttribute is null
                ? assembly.GetName().Version?.ToString()
                : assemblyVersionAttribute.InformationalVersion;
        }
    }
}