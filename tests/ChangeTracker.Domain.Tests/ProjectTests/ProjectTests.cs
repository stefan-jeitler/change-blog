using System;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Tests.TestDoubles;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ProjectTests
{
    public class ProjectTests
    {
        private Guid _testAccountId;
        private DateTime? _testClosedDate;
        private DateTime _testCreationDate;
        private Guid _testId;
        private Name _testName;
        private VersioningScheme _testVersioningScheme;

        public ProjectTests()
        {
            _testId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
            _testName = Name.Parse("ProjectX");
            _testCreationDate = DateTime.Parse("2021-04-03");
            _testAccountId = TestAccount.Id;
            _testVersioningScheme = TestAccount.CustomVersioningScheme;
            _testClosedDate = null;
        }

        private Project CreateProject() => new(_testId, _testAccountId, _testName, _testVersioningScheme,
            _testCreationDate, _testClosedDate);

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var project = CreateProject();

            project.Id.Should().Be(_testId);
            project.AccountId.Should().Be(_testAccountId);
            project.Name.Should().Be(_testName);
            project.VersioningScheme.Should().Be(_testVersioningScheme);
            project.CreatedAt.Should().Be(_testCreationDate);
            project.ClosedAt.HasValue.Should().BeFalse();
        }

        [Fact]
        public void Create_WithClosedAtDate_DateProperlySet()
        {
            _testClosedDate = DateTime.Parse("2021-04-16");

            var project = CreateProject();

            project.ClosedAt.Should().HaveValue();
            project.ClosedAt!.Value.Should().Be(_testClosedDate!.Value);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            _testId = Guid.Empty;

            Func<Project> act = CreateProject;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            _testAccountId = Guid.Empty;

            Func<Project> act = CreateProject;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            _testName = null;

            Func<Project> act = CreateProject;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullVersioningScheme_ArgumentException()
        {
            _testVersioningScheme = null;

            Func<Project> act = CreateProject;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreatedAtDate_ArgumentException(string invalidDate)
        {
            _testCreationDate = DateTime.Parse(invalidDate);

            Func<Project> act = CreateProject;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidClosedAtDate_ArgumentException(string invalidDate)
        {
            _testClosedDate = DateTime.Parse(invalidDate);

            Func<Project> act = CreateProject;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}