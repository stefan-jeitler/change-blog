using System;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests.VersionTests
{
    public class VersioningSchemeTest
    {
        private Guid? _testAccountId;
        private DateTime _testCreationDate;
        private DateTime? _testDeletionDate;
        private Text _testDescription;
        private Guid _testId;
        private Name _testName;
        private Text _testRegexPatter;
        private Guid _testUserId;

        public VersioningSchemeTest()
        {
            _testId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");
            _testName = Name.Parse("Custom");
            _testRegexPatter = Text.Parse(@"^(\d+\.)?(\d+\.)?(\*|\d+)$");
            _testAccountId = Guid.Parse("fce44eb5-b842-44ca-b1c7-7a7cb88ccd3d");
            _testDescription = Text.Parse("custom scheme for my needs.");
            _testCreationDate = DateTime.Parse("2021-04-03");
            _testUserId = Guid.Parse("fc573542-e3bd-4454-9b56-37e7bbd9c349");
            _testDeletionDate = null;
        }

        private VersioningScheme CreateScheme() =>
            new(_testId, _testName, _testRegexPatter,
                _testDescription, _testAccountId, _testUserId, _testDeletionDate, _testCreationDate);

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var vs = CreateScheme();

            vs.Id.Should().Be(_testId);
            vs.Name.Should().Be(_testName);
            vs.RegexPattern.Should().Be(_testRegexPatter);
            vs.AccountId.Should().HaveValue();
            vs.AccountId!.Value.Should().Be(_testAccountId!.Value);
            vs.Description.Should().Be(_testDescription);
            vs.CreatedAt.Should().Be(_testCreationDate);
            vs.CreatedByUser.Should().Be(_testUserId);
            vs.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            _testId = Guid.Empty;

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            _testName = null;

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullRegexPattern_ArgumentNullException()
        {
            _testRegexPatter = null;

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullAccountId_NoAccountId()
        {
            _testAccountId = null;

            var scheme = CreateScheme();

            scheme.AccountId.HasValue.Should().BeFalse();
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            _testAccountId = Guid.Empty;

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyUserIdId_ArgumentException()
        {
            _testUserId = Guid.Empty;

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullDescription_ArgumentNullException()
        {
            _testDescription = null;

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreationDate_ArgumentException(string invalidDate)
        {
            _testCreationDate = DateTime.Parse(invalidDate);

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletionDate_ArgumentException(string invalidDate)
        {
            _testDeletionDate = DateTime.Parse(invalidDate);

            Func<VersioningScheme> act = CreateScheme;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
