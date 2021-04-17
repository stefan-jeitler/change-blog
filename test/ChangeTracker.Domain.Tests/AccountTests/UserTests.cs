﻿using System;
using ChangeTracker.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.AccountTests
{
    public class UserTests
    {
        private Guid _testUserId;
        private Email _testEmail;
        private Name _testFirstName;
        private Name _testLastName;
        private DateTime _testDeletionDate;

        public UserTests()
        {
            _testUserId = Guid.Parse("315dc849-7c17-4268-93a1-40b7ab337c68");
            _testEmail = Email.Parse("stefan@changeLog");
            _testFirstName = Name.Parse("Stefan");
            _testLastName = Name.Parse("Jeitler");
            _testDeletionDate = DateTime.Parse("2021-04-03");
        }

        private User CreateUser() => new(_testUserId, _testEmail, _testFirstName, _testLastName, _testDeletionDate);

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var user = CreateUser();

            user.Id.Should().Be(_testUserId);
            user.Email.Should().Be(_testEmail);
            user.FirstName.Should().Be(_testFirstName);
            user.LastName.Should().Be(_testLastName);
            user.DeletedAt.Should().Be(_testDeletionDate);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            _testUserId = Guid.Empty;

            Func<User> act = CreateUser;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullEmail_ArgumentNullException()
        {
            _testEmail = null;

            Func<User> act = CreateUser;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullFirstName_ArgumentNullException()
        {
            _testFirstName = null;

            Func<User> act = CreateUser;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullLastName_ArgumentNullException()
        {
            _testLastName = null;

            Func<User> act = CreateUser;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidDeletionDate_ArgumentException(string invalidDate)
        {
            _testDeletionDate = DateTime.Parse(invalidDate);

            Func<User> act = CreateUser;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}