using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ChangeTracker.DataAccess.Postgres.Tests
{
    public class Configuration
    {
        public static Lazy<IConfiguration> Instance = new(BuildConfiguration);

        public static string ConnectionString => Instance.Value.GetConnectionString("ChangeTrackerDb");

        private static IConfiguration BuildConfiguration() =>
            new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false)
                .AddUserSecrets<Configuration>()
                .Build();
    }
}
