using System;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ProjectTests
{
    public class ProjectUserRoleTests
    {
        private static readonly Guid TestUserId = Guid.Parse("52596382-2bed-435e-b3f9-dc5e5073823e");
        private static readonly Guid TestProjectId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
        private static readonly Guid TestRoleId = Guid.Parse("50b5948a-20b1-4687-ad42-80e179394ab5");

        [Fact]
        public void Create_WitValidArguments_Successful()
        {
            var projectUserRole = new ProjectUserRole(TestUserId, TestProjectId, TestRoleId);

            projectUserRole.UserId.Should().Be(TestUserId);
            projectUserRole.ProjectId.Should().Be(TestProjectId);
            projectUserRole.RoleId.Should().Be(TestRoleId);
        }

        [Fact]
        public void Create_WithEmptyUserId_ArgumentException()
        {
            Func<ProjectUserRole> act = () => new ProjectUserRole(Guid.Empty, TestProjectId, TestRoleId);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            Func<ProjectUserRole> act = () => new ProjectUserRole(TestUserId, Guid.Empty, TestRoleId);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyRoleId_ArgumentException()
        {
            Func<ProjectUserRole> act = () => new ProjectUserRole(TestUserId, TestProjectId, Guid.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}