using System.Threading.Tasks;
using ChangeTracker.DataAccess.Postgres;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChangeTracker.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await VerifyDbSchemaVersionAsync(host);
            await host.RunAsync();
        }

        private static async Task VerifyDbSchemaVersionAsync(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var schemaVersion = scope.ServiceProvider.GetRequiredService<SchemaVersion>();
            await schemaVersion.VerifySchemaVersionAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
                .ConfigureAppConfiguration((_, config) => { config.AddEnvironmentVariables("POSTGRESQLCONNSTR_"); });
        }
    }
}