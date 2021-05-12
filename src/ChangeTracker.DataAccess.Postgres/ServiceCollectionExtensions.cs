using System;
using System.Data;
using ChangeTracker.Application.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ChangeTracker.DataAccess.Postgres
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresDataAccess(this IServiceCollection services,
            string connectionString) =>
            services
                .AddDbSession(connectionString)
                .AddScoped<UserAccessDao>()
                .AddScoped<SchemaVersion>();

        private static IServiceCollection AddDbSession(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<Func<IDbConnection>>(_ => () => new NpgsqlConnection(connectionString));

            services.AddScoped(_ => new LazyDbConnection(() => new NpgsqlConnection(connectionString)));
            services.AddScoped<DbSession>();

            services.AddScoped<IDbAccessor>(sp => sp.GetRequiredService<DbSession>());
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DbSession>());

            return services;
        }
    }
}