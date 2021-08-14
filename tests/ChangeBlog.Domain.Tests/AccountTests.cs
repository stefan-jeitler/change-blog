using System;
using ChangeBlog.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests
{
    public class AccountTests
    {
        private Guid? _defaultVersioningSchemeId;
        private Guid _testAccountId;
        private DateTime _testCreationDate;
        private DateTime _testDeletionDate;
        private Name _testName;

        public AccountTests()
        {
            _testAccountId = Guid.Parse("c1b588ee-069d-453e-8f74-fc43b7ae0649");
            _testName = Name.Parse("Account X");
            _defaultVersioningSchemeId = Guid.Parse("042b3384-a149-4739-b281-89e51cbcf549");
            _testCreationDate = DateTime.Parse("2021-04-03");
            _testDeletionDate = DateTime.Parse("2021-04-03");
        }

        private Account CreateAccount() =>
            new(_testAccountId,
                _testName,
                _defaultVersioningSchemeId,
                _testCreationDate,
                _testDeletionDate);

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var account = CreateAccount();

            account.Id.Should().Be(_testAccountId);
            account.Name.Should().Be(_testName);
            account.DefaultVersioningSchemeId.Should().HaveValue();
            account.DefaultVersioningSchemeId!.Value.Should().Be(_defaultVersioningSchemeId!.Value);
            account.CreatedAt.Should().Be(_testCreationDate);
            account.DeletedAt.Should().Be(_testDeletionDate);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            _testAccountId = Guid.Empty;

            Func<Account> act = CreateAccount;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            _testName = null;

            Func<Account> act = CreateAccount;

            act.Should().ThrowExactly<ArgumentNullException>();
        }


        [Fact]
        public void Create_WithNullVersioningSchemeId_NullIsAllowed()
        {
            _defaultVersioningSchemeId = null;

            var account = CreateAccount();

            account.DefaultVersioningSchemeId.HasValue.Should().BeFalse();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreationDate_ArgumentException(string invalidDate)
        {
            _testCreationDate = DateTime.Parse(invalidDate);

            Func<Account> act = CreateAccount;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletionDate_ArgumentException(string invalidDate)
        {
            _testDeletionDate = DateTime.Parse(invalidDate);

            Func<Account> act = CreateAccount;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
