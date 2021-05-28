using System;
using System.Data;
using System.Runtime.CompilerServices;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Application.DataAccess.ChangeLogs;
using ChangeTracker.Application.DataAccess.Products;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.DataAccess.Versions;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Account;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.ChangeLogs;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Product;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.User;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Version;
using ChangeTracker.DataAccess.Postgres.TypeHandler;
using Dapper;
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
                .AddScoped<SchemaVersion>()
                .AddDataAccessObjects();

        [ModuleInitializer]
        public static void RegisterTypeHandler()
        {
            SqlMapper.AddTypeHandler(new NameTypeHandler());
            SqlMapper.AddTypeHandler(new OptionalNameTypeHandler());
            SqlMapper.AddTypeHandler(new TextTypeHandler());
            SqlMapper.AddTypeHandler(new EmailTypeHandler());
            SqlMapper.AddTypeHandler(new ChangeLogTextTypeHandler());
            SqlMapper.AddTypeHandler(new ClVersionValueTypeHandler());
            SqlMapper.AddTypeHandler(new LabelsTypeHandler());
            SqlMapper.AddTypeHandler(new IssuesTypeHandler());
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

        private static IServiceCollection AddDataAccessObjects(this IServiceCollection services) =>
            services
                .AddScoped<IAccountDao, AccountDao>()
                .AddScoped<IProductDao, ProductDao>()
                .AddScoped<IVersioningSchemeDao, VersioningSchemeDao>()
                .AddScoped<IUserDao, UserDao>()
                .AddScoped<IRolesDao, RolesDao>()
                .AddScoped<IVersionDao, VersionDao>()
                .AddScoped<IChangeLogCommandsDao, ChangeLogCommandsDao>()
                .AddScoped<IChangeLogQueriesDao, ChangeLogQueriesDao>();
    }
}