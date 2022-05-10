using System;
using System.Data;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Boundaries.DataAccess.Versions;
using ChangeBlog.Application.Proxies;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Account;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.ChangeLog;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Versions;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace ChangeBlog.DataAccess.Postgres;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresDataAccess(this IServiceCollection services,
        string connectionString)
    {
        DapperTypeHandlers.Add();

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
            .AddScoped<IUserApiKeysDao, UserApiKeysDao>()
            .AddScoped<IRolesDao, RolesDao>()
            .AddScoped<IVersionDao, VersionDao>()
            .AddScoped<IChangeLogQueriesDao, ChangeLogQueriesDao>();

        services.AddScoped<IChangeLogCommandsDao, ChangeLogCommandsDao>()
            .Decorate<IChangeLogCommandsDao, ChangeLogLineReadonlyCheckProxy>();

        return services;
    }
}