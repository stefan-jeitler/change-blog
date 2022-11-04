using System;
using System.Threading.Tasks;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users.UserAccess;
using ChangeBlog.Domain;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeBlog.DataAccess.Postgres.Tests.DataAccessObjectsTests;

public class UserAccessDaoTest
{
    private static UserAccessDao CreateDao()
    {
        return new UserAccessDao(() => new NpgsqlConnection(Configuration.ConnectionString),
            NullLogger<UserAccessDao>.Instance);
    }

    [Fact]
    public async Task FindUserId_ExistingUser_ReturnsUserId()
    {
        var userAccessDao = CreateDao();
        var t_ua_account_01_user_01 = Guid.Parse("f575503e-4eee-4d6d-b2c1-f11d8fc3da76");

        var userId = await userAccessDao.FindActiveUserIdByApiKeyAsync("acc01usr01");

        userId.Should().HaveValue();
        userId.Should().Be(t_ua_account_01_user_01);
    }

    [Fact]
    public async Task FindUserId_NotExistingUser_ReturnsNull()
    {
        var userAccessDao = CreateDao();

        var userId = await userAccessDao.FindActiveUserIdByApiKeyAsync("not-existing-api-key");

        userId.Should().NotHaveValue();
    }

    [Fact]
    public async Task GetAccountRoles_DefaultUser_ReturnsDefaultUserRole()
    {
        var userAccessDao = CreateDao();
        var t_ua_account_01_user_01 = Guid.Parse("f575503e-4eee-4d6d-b2c1-f11d8fc3da76");
        var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

        var accountRoles = (await userAccessDao.GetAccountRolesAsync(t_ua_account_01_user_01, t_ua_account_01))
            .AsList();

        accountRoles.Should().HaveCount(1);
        accountRoles.Should().Contain(x => x.Name.Value == Role.DefaultUser);
    }

    [Fact]
    public async Task GetAccountRoles_DefaultUserAndPlatformManager_ReturnsDefaultUserAndPlatformManagerRole()
    {
        var userAccessDao = CreateDao();
        var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
        var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

        var accountRoles = (await userAccessDao.GetAccountRolesAsync(t_ua_account_01_user_02, t_ua_account_01))
            .AsList();

        accountRoles.Should().HaveCount(2);
        accountRoles.Should().Contain(x => x.Name.Value == Role.DefaultUser);
        accountRoles.Should().Contain(x => x.Name.Value == Role.PlatformManager);
    }

    [Fact]
    public async Task GetAccountAndProductRoles_NoProductRolesExists_ReturnsEmptyProductRoles()
    {
        var userAccessDao = CreateDao();
        var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
        var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

        var (_, productRoles) =
            await userAccessDao.GetRolesByProductIdAsync(t_ua_account_01_user_02, t_ua_account_01_proj_02);

        productRoles.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAccountAndProductRoles_ProductRoleExists_ReturnsProductRole()
    {
        var userAccessDao = CreateDao();
        var t_ua_account_01_proj_01 = Guid.Parse("139a2e54-e9be-4168-98b4-2839d9b3db04");
        var t_ua_account_01_user_01 = Guid.Parse("f575503e-4eee-4d6d-b2c1-f11d8fc3da76");

        var (_, productRoles) =
            await userAccessDao.GetRolesByProductIdAsync(t_ua_account_01_user_01, t_ua_account_01_proj_01);

        productRoles.Should().ContainSingle(x => x.Name.Value == Role.ProductManager);
    }
}