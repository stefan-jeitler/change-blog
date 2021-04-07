using System;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Tests.TestDoubles;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ProjectTests
{
    public class ProjectTests
    {
        private static readonly Guid TestId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
        private static readonly Name TestName = Name.Parse("ProjectX");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04-03");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var project = new Project(TestId,
                TestAccount.Id,
                TestName,
                TestAccount.CustomVersioningScheme,
                TestCreationDate,
                null);

            project.Id.Should().Be(TestId);
            project.AccountId.Should().Be(TestAccount.Id);
            project.Name.Should().Be(TestName);
            project.VersioningScheme.Should().Be(TestAccount.CustomVersioningScheme);
            project.CreatedAt.Should().Be(TestCreationDate);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            Func<Project> act = () => new Project(Guid.Empty,
                TestAccount.Id,
                TestName,
                TestAccount.CustomVersioningScheme,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            Func<Project> act = () => new Project(TestId,
                Guid.Empty,
                TestName,
                TestAccount.CustomVersioningScheme,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            Func<Project> act = () => new Project(TestId,
                TestAccount.Id,
                null,
                TestAccount.CustomVersioningScheme,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullVersioningScheme_ArgumentException()
        {
            Func<Project> act = () => new Project(TestId,
                TestAccount.Id,
                TestName,
                null,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreationDate_ArgumentException(string createdAt)
        {
            Func<Project> act = () => new Project(TestId,
                TestAccount.Id,
                TestName,
                TestAccount.CustomVersioningScheme,
                DateTime.Parse(createdAt),
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletionDate_ArgumentException(string deletedAt)
        {
            Func<Project> act = () => new Project(TestId,
                TestAccount.Id,
                TestName,
                TestAccount.CustomVersioningScheme,
                TestCreationDate,
                DateTime.Parse(deletedAt));

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}