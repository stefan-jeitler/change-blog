using System;
using Microsoft.Extensions.Configuration;

namespace ChangeTracker.DataAccess.Postgres.Tests
{
    public class Configuration
    {
        public static Lazy<IConfiguration> Instance = new(BuildConfiguration);

        public static string ConnectionString => Instance.Value.GetConnectionString("ChangeTrackerDb");

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Configuration>()
                .AddEnvironmentVariables()
                .Build();
        }
    }
}