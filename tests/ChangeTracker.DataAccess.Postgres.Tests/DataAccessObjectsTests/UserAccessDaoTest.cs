using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
using FluentAssertions;
using Npgsql;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.DataAccess.Postgres.Tests.DataAccessObjectsTests
{
    public class UserAccessDaoTest
    {
        private static UserAccessDao CreateDao()
        {
            return new(() => new NpgsqlConnection(Configuration.ConnectionString));
        }

        [Fact]
        public async Task FindUserId_ExistingUser_ReturnsUserId()
        {
            var userAccessDao = CreateDao();
            var t_ua_account_01_user_01 = Guid.Parse("f575503e-4eee-4d6d-b2c1-f11d8fc3da76");

            var userId = await userAccessDao.FindUserIdAsync("acc01usr01");

            userId.Should().HaveValue();
            userId.Should().Be(t_ua_account_01_user_01);
        }

        [Fact]
        public async Task FindUserId_NotExistingUser_ReturnsNull()
        {
            var userAccessDao = CreateDao();

            var userId = await userAccessDao.FindUserIdAsync("not-existing-api-key");

            userId.Should().NotHaveValue();
        }

        [Fact]
        public async Task HasAccountPermission_DefaultUser_ReturnsFalse()
        {
            var userAccessDao = CreateDao();
            var t_ua_account_01_user_01 = Guid.Parse("f575503e-4eee-4d6d-b2c1-f11d8fc3da76");
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

            var hasPermission =
                await userAccessDao.HasAccountPermissionAsync(t_ua_account_01_user_01, t_ua_account_01,
                    Permission.ViewAccountUsers);

            hasPermission.Should().BeFalse();
        }

        [Fact]
        public async Task HasAccountPermission_PlatformManager_ReturnsTrue()
        {
            var userAccessDao = CreateDao();
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

            var hasPermission =
                await userAccessDao.HasAccountPermissionAsync(t_ua_account_01_user_02, t_ua_account_01,
                    Permission.ViewAccountUsers);

            hasPermission.Should().BeTrue();
        }

        [Fact]
        public async Task HasProductPermission_PlatformManager_ReturnsTrue()
        {
            var userAccessDao = CreateDao();
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

            var hasPermission =
                await userAccessDao.HasProductPermissionAsync(t_ua_account_01_user_02, t_ua_account_01_proj_02,
                    Permission.ViewChangeLogLines);

            hasPermission.Should().BeTrue();
        }

        [Fact]
        public async Task HasProductPermission_DefaultUser_ReturnsFalse()
        {
            var userAccessDao = CreateDao();
            var t_ua_account_01_user_01 = Guid.Parse("f575503e-4eee-4d6d-b2c1-f11d8fc3da76");
            var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

            var hasPermission =
                await userAccessDao.HasProductPermissionAsync(t_ua_account_01_user_01, t_ua_account_01_proj_02,
                    Permission.ViewChangeLogLines);

            hasPermission.Should().BeFalse();
        }
    }
}