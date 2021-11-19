using System;
using System.Threading.Tasks;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Account;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Xunit;

namespace ChangeBlog.DataAccess.Postgres.Tests.DataAccessObjectsTests;

public class RolesDaoTests : IDisposable
{
    private readonly LazyDbConnection _lazyDbConnection;

    public RolesDaoTests()
    {
        _lazyDbConnection = new LazyDbConnection(() => new NpgsqlConnection(Configuration.ConnectionString));
    }

    public void Dispose()
    {
        _lazyDbConnection?.Dispose();
    }

    [Fact]
    public async Task GetRoles_HappyPath_ReturnsRoles()
    {
        var dbSession = new DbSession(_lazyDbConnection);
        var rolesDao = new RolesDao(dbSession, NullLogger<RolesDao>.Instance);

        var roles = await rolesDao.GetRolesAsync();

        roles.Should().NotBeEmpty();
    }
}