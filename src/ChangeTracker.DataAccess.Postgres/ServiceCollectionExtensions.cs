using System;
using System.Data;
using System.Runtime.CompilerServices;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
using ChangeTracker.DataAccess.Postgres.TypeHandler;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ChangeTracker.DataAccess.Postgres
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresDataAccess(this IServiceCollection services,
            string connectionString)
        {
            return services
                .AddDbSession(connectionString)
                .AddScoped<UserAccessDao>()
                .AddScoped<SchemaVersion>()
                .AddDataAccessObjects();
        }

        [ModuleInitializer]
        public static void RegisterTypeHandler()
        {
            SqlMapper.AddTypeHandler(new NameTypeHandler());
            SqlMapper.AddTypeHandler(new TextTypeHandler());
            SqlMapper.AddTypeHandler(new EmailTypeHandler());
            SqlMapper.AddTypeHandler(new ChangeLogTextTypeHandler());
            SqlMapper.AddTypeHandler(new ClVersionValueTypeHandler());
        }

        private static IServiceCollection AddDbSession(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton<Func<IDbConnection>>(_ => () => new NpgsqlConnection(connectionString));

            services.AddScoped(_ => new LazyDbConnection(() => new NpgsqlConnection(connectionString)));
            services.AddScoped<DbSession>();

            services.AddScoped<IDbAccessor>(sp => sp.GetRequiredService<DbSession>());
            services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<DbSession>());

            return services;
        }

        private static IServiceCollection AddDataAccessObjects(this IServiceCollection services)
        {
            return services
                .AddScoped<IAccountDao, AccountDao>()
                .AddScoped<IProjectDao, ProjectDao>()
                .AddScoped<IVersioningSchemeDao, VersioningSchemeDao>()
                .AddScoped<IUserDao, UserDao>()
                .AddScoped<IRolesDao, RolesDao>();
        }
    }
}