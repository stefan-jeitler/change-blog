using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Domain.Authorization;
using ChangeBlog.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests.Authorization
{
    public class AuthorizationServiceTests
    {
        private static readonly Role TestDefaultUserRole =
            new(Guid.Parse("8ec8ae99-83d7-4958-9df5-72eb8eaf002b"),
                Name.Parse(Role.DefaultUser),
                Text.Parse("Basic User"),
                DateTime.Parse("2021-05-21"),
                new[] {Permission.ViewOwnUser, Permission.ViewAccount});

        private static readonly Role TestDeveloperUserRole =
            new(Guid.Parse("ad7b83ed-8fce-4341-978b-8d1eae66f346"),
                Name.Parse(Role.Developer),
                Text.Parse("Developer"),
                DateTime.Parse("2021-05-21"),
                new[]
                {
                    Permission.ViewOwnUser, Permission.ViewAccount, Permission.AddOrUpdateProduct,
                    Permission.ViewChangeLogLines
                });

        private static readonly Role TestProductOwnerUserRole =
            new(Guid.Parse("ad7b83ed-8fce-4341-978b-8d1eae66f346"),
                Name.Parse(Role.ProductOwner),
                Text.Parse("Product Owner"),
                DateTime.Parse("2021-05-21"),
                new[]
                {
                    Permission.ViewOwnUser, Permission.ViewAccount, Permission.AddOrUpdateProduct,
                    Permission.ViewChangeLogLines, Permission.AddOrUpdateChangeLogLine, Permission.AddOrUpdateVersion
                });


        [Fact]
        public void Create_WithNullAccountPermissions_ArgumentNullException()
        {
            Func<AuthorizationService> act = () => new AuthorizationService(null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }


        [Fact]
        public void Create_WithNullProductPermissions_ArgumentNullException()
        {
            Func<AuthorizationService> act = () => new AuthorizationService(Enumerable.Empty<Role>(), null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }


        [Fact]
        public void GetAuthorizationState_NoAccountPermissionsExists_Inaccessible()
        {
            var authService = new AuthorizationService(Enumerable.Empty<Role>());

            var authorizationState = authService.GetAuthorizationState(Permission.AddVersion);

            authorizationState.Should().Be(AuthorizationState.Inaccessible);
        }

        [Fact]
        public void GetAuthorizationState_NoAccountDefaultUserRole_Inaccessible()
        {
            var accountRoles = new List<Role> {TestDeveloperUserRole};
            var authService = new AuthorizationService(accountRoles);

            var authorizationState = authService.GetAuthorizationState(Permission.AddVersion);

            authorizationState.Should().Be(AuthorizationState.Inaccessible);
        }

        [Fact]
        public void GetAuthorizationState_MatchingAccountPermission_Authorized()
        {
            var accountRoles = new List<Role> {TestDefaultUserRole, TestDeveloperUserRole};
            var authService = new AuthorizationService(accountRoles);

            var authorizationState = authService.GetAuthorizationState(Permission.ViewChangeLogLines);

            authorizationState.Should().Be(AuthorizationState.Authorized);
        }

        [Fact]
        public void GetAuthorizationState_NoAccountPermissionAndNoProductPermissions_Inaccessible()
        {
            var authService = new AuthorizationService(Enumerable.Empty<Role>(), Enumerable.Empty<Role>());

            var authorizationState = authService.GetAuthorizationState(Permission.ViewOwnUser);

            authorizationState.Should().Be(AuthorizationState.Inaccessible);
        }

        [Fact]
        public void GetAuthorizationState_MatchingAccountPermissionAndNoMatchingProductPermission_Authorized()
        {
            var accountRoles = new List<Role> {TestDefaultUserRole, TestProductOwnerUserRole};
            var productRoles = new List<Role> {TestDeveloperUserRole};
            var authService = new AuthorizationService(accountRoles, productRoles);

            var authorizationState = authService.GetAuthorizationState(Permission.AddOrUpdateVersion);

            authorizationState.Should().Be(AuthorizationState.Unauthorized);
        }

        [Fact]
        public void GetAuthorizationState_ExistingMatchingAccountPermissionAndMatchingProductPermission_Authorized()
        {
            var accountRoles = new List<Role> {TestDefaultUserRole, TestDeveloperUserRole};
            var productRoles = new List<Role> {TestDeveloperUserRole};
            var authService = new AuthorizationService(accountRoles, productRoles);

            var authorizationState = authService.GetAuthorizationState(Permission.AddOrUpdateProduct);

            authorizationState.Should().Be(AuthorizationState.Authorized);
        }
    }
}
