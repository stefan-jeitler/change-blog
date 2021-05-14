using System;
using System.Threading.Tasks;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace ChangeTracker.DataAccess.Postgres.Tests.DataAccessObjectsTests
{
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
            var rolesDao = new RolesDao(dbSession);

            var roles = await rolesDao.GetRolesAsync();

            roles.Should().NotBeEmpty();
        }
    }
}