using System;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.VersionTests
{
    public class VersioningSchemeTest
    {
        private static readonly Guid TestId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");
        private static readonly Name TestName = Name.Parse("Custom");
        private static readonly Text TestRegexPatter = Text.Parse(@"^(\d+\.)?(\d+\.)?(\*|\d+)$");
        private static readonly Guid TestAccountId = Guid.Parse("fce44eb5-b842-44ca-b1c7-7a7cb88ccd3d");
        private static readonly Text TestDescription = Text.Parse("custom scheme for my needs.");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04-03");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var vs = new VersioningScheme(TestId,
                TestName,
                TestRegexPatter,
                TestAccountId,
                TestDescription,
                TestCreationDate,
                null);

            vs.Id.Should().Be(TestId);
            vs.Name.Should().Be(TestName);
            vs.RegexPattern.Should().Be(TestRegexPatter);
            vs.AccountId.HasValue.Should().BeTrue();
            vs.AccountId.Value.Should().Be(TestAccountId);
            vs.Description.Should().Be(TestDescription);
            vs.CreatedAt.Should().Be(TestCreationDate);
            vs.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            Func<VersioningScheme> act = () => new VersioningScheme(Guid.Empty,
                TestName,
                TestRegexPatter,
                TestAccountId,
                TestDescription,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            Func<VersioningScheme> act = () => new VersioningScheme(TestId,
                null,
                TestRegexPatter,
                TestAccountId,
                TestDescription,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullRegexPattern_ArgumentNullException()
        {
            Func<VersioningScheme> act = () => new VersioningScheme(TestId,
                TestName,
                null,
                TestAccountId,
                TestDescription,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            Func<VersioningScheme> act = () => new VersioningScheme(TestId,
                TestName,
                TestRegexPatter,
                Guid.Empty,
                TestDescription,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullDescription_ArgumentNullException()
        {
            Func<VersioningScheme> act = () => new VersioningScheme(TestId,
                TestName,
                TestRegexPatter,
                TestAccountId,
                null,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreationDate_ArgumentException(string invalidDate)
        {
            Func<VersioningScheme> act = () => new VersioningScheme(TestId,
                TestName,
                TestRegexPatter,
                TestAccountId,
                TestDescription,
                DateTime.Parse(invalidDate),
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletionDate_ArgumentException(string invalidDate)
        {
            Func<VersioningScheme> act = () => new VersioningScheme(TestId,
                TestName,
                TestRegexPatter,
                TestAccountId,
                TestDescription,
                TestCreationDate,
                DateTime.Parse(invalidDate));

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}