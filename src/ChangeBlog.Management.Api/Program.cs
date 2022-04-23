using System.Threading.Tasks;
using ChangeBlog.DataAccess.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChangeBlog.Management.Api;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        await ApproveDbSchemaVersionAsync(host);
        await host.RunAsync();
    }
    
    private static async Task ApproveDbSchemaVersionAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var schemaVersion = scope.ServiceProvider.GetRequiredService<SchemaVersion>();
        await schemaVersion.ApproveAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            // https://docs.microsoft.com/en-us/azure/app-service/reference-app-settings?tabs=kudu%2Cdotnet#variable-prefixes
            .ConfigureAppConfiguration((_, config) => { config.AddEnvironmentVariables("POSTGRESQLCONNSTR_"); });
    }
}