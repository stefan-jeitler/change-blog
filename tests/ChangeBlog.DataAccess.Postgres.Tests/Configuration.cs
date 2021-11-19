using System;
using Microsoft.Extensions.Configuration;

namespace ChangeBlog.DataAccess.Postgres.Tests;

public class Configuration
{
    public static Lazy<IConfiguration> Instance = new(BuildConfiguration);

    public static string ConnectionString => Instance.Value.GetConnectionString("ChangeBlogDb");

    private static IConfiguration BuildConfiguration() =>
        new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false)
            .AddUserSecrets<Configuration>(true)
            .AddEnvironmentVariables()
            .Build();
}