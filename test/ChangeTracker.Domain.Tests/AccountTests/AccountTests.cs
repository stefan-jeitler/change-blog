using System;
using ChangeTracker.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.AccountTests
{
    public class AccountTests
    {
        private static readonly Guid TestAccountId = Guid.Parse("c1b588ee-069d-453e-8f74-fc43b7ae0649");
        private static readonly Name TestAccountName = Name.Parse("Account X");
        private static readonly Guid TestDefaultVersioningSchemeId = Guid.Parse("042b3384-a149-4739-b281-89e51cbcf549");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04-03");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var account = new Account(TestAccountId,
                TestAccountName,
                TestDefaultVersioningSchemeId,
                TestCreationDate,
                null);

            account.Id.Should().Be(TestAccountId);
            account.Name.Should().Be(TestAccountName);
            account.DefaultVersioningSchemeId.HasValue.Should().BeTrue();
            account.DefaultVersioningSchemeId.Value.Should().Be(TestDefaultVersioningSchemeId);
            account.CreatedAt.Should().Be(TestCreationDate);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            Func<Account> act = () => new Account(Guid.Empty,
                TestAccountName,
                TestDefaultVersioningSchemeId,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            Func<Account> act = () => new Account(TestAccountId,
                null,
                TestDefaultVersioningSchemeId,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }


        [Fact]
        public void Create_WithNullVersioningSchemeId_NullIsAllowed()
        {
            var account = new Account(TestAccountId, TestAccountName, null,
                TestCreationDate,
                null);

            account.DefaultVersioningSchemeId.HasValue.Should().BeFalse();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreationDate_ArgumentException(string invalidDate)
        {
            Func<Account> act = () => new Account(TestAccountId,
                TestAccountName,
                TestDefaultVersioningSchemeId,
                DateTime.Parse(invalidDate),
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletionDate_ArgumentException(string invalidDate)
        {
            Func<Account> act = () => new Account(TestAccountId,
                TestAccountName,
                TestDefaultVersioningSchemeId,
                TestCreationDate,
                DateTime.Parse(invalidDate));

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}