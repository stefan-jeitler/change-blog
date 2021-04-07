using System;
using ChangeTracker.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.AccountTests
{
    public class UserTests
    {
        private static readonly Guid TestUserId = Guid.Parse("315dc849-7c17-4268-93a1-40b7ab337c68");
        private static readonly Email TestEmail = Email.Parse("stefan@changeLog");
        private static readonly Name TestFirstName = Name.Parse("Stefan");
        private static readonly Name TestLastName = Name.Parse("Jeitler");
        private static readonly DateTime TestDeletionDate = DateTime.Parse("2021-04-03");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var user = new User(TestUserId,
                TestEmail,
                TestFirstName,
                TestLastName,
                TestDeletionDate);

            user.Id.Should().Be(TestUserId);
            user.Email.Should().Be(TestEmail);
            user.FirstName.Should().Be(TestFirstName);
            user.LastName.Should().Be(TestLastName);
            user.DeletedAt.Should().Be(TestDeletionDate);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            Func<User> act = () => new User(Guid.Empty,
                TestEmail,
                TestFirstName,
                TestLastName,
                TestDeletionDate);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullEmail_ArgumentNullException()
        {
            Func<User> act = () => new User(TestUserId,
                null,
                TestFirstName,
                TestLastName,
                TestDeletionDate);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullFirstName_ArgumentNullException()
        {
            Func<User> act = () => new User(TestUserId,
                TestEmail,
                null,
                TestLastName,
                TestDeletionDate);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullLastName_ArgumentNullException()
        {
            Func<User> act = () => new User(TestUserId,
                TestEmail,
                TestFirstName,
                null,
                TestDeletionDate);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletionDate_ArgumentException(string invalidDate)
        {
            Func<User> act = () => new User(TestUserId,
                TestEmail,
                TestFirstName,
                TestLastName,
                DateTime.Parse(invalidDate));

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}