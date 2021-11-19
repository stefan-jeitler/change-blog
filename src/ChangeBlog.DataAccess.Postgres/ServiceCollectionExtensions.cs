using System;
using System.Data;
using System.Runtime.CompilerServices;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.Accounts;
using ChangeBlog.Application.DataAccess.ChangeLog;
using ChangeBlog.Application.DataAccess.Products;
using ChangeBlog.Application.DataAccess.Users;
using ChangeBlog.Application.DataAccess.Versions;
using ChangeBlog.Application.Proxies;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Account;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.ChangeLog;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Versions;
using ChangeBlog.DataAccess.Postgres.TypeHandler;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ChangeBlog.DataAccess.Postgres;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresDataAccess(this IServiceCollection services,
        string connectionString)
    {
        DapperTypeHandlers.Initialize();

        return services
            .AddDbSession(connectionString)
            .AddScoped<UserAccessDao>()
            .AddScoped<IUserAccessDao>(sp => sp.GetRequiredService<UserAccessDao>())
            .AddScoped<SchemaVersion>()
            .AddDataAccessObjects();
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
        services
            .AddScoped<IAccountDao, AccountDao>()
            .AddScoped<IProductDao, ProductDao>()
            .AddScoped<IVersioningSchemeDao, VersioningSchemeDao>()
            .AddScoped<IUserDao, UserDao>()
            .AddScoped<IRolesDao, RolesDao>()
            .AddScoped<IVersionDao, VersionDao>()
            .AddScoped<IChangeLogQueriesDao, ChangeLogQueriesDao>();

        services.AddScoped<IChangeLogCommandsDao, ChangeLogCommandsDao>()
            .Decorate<IChangeLogCommandsDao, ChangeLogLineReadonlyCheckProxy>();

        return services;
    }
}