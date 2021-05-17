using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.Common;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests
{
    public class RoleTests
    {
        private DateTime _testCreationDate;
        private Text _testDescription;
        private Guid _testId;
        private Name _testName;
        private IList<Name> _testPermissions;

        public RoleTests()
        {
            _testId = Guid.Parse("a93eb48d-277a-4a32-8f0b-0d9ace632b11");
            _testName = Name.Parse("Tester");
            _testDescription = Text.Parse("Tester role.");
            _testCreationDate = DateTime.Parse("2021-05-14");
            _testPermissions = new List<Name>(2) {Name.Parse("TestApi"), Name.Parse("TestUi")};
        }

        private Role CreateRole()
        {
            return new(_testId, _testName, _testDescription, _testCreationDate, _testPermissions);
        }

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var role = CreateRole();

            role.Id.Should().Be(_testId);
            role.Name.Should().Be(_testName);
            role.Description.Should().Be(_testDescription);
            role.CreatedAt.Should().Be(_testCreationDate);
            role.Permissions.Should().Contain(_testPermissions.First());
            role.Permissions.Should().Contain(_testPermissions.Last());
            role.Permissions.Count.Should().Be(2);
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            _testId = Guid.Empty;

            Func<Role> act = CreateRole;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullName_ArgumentNullException()
        {
            _testName = null;

            Func<Role> act = CreateRole;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullDescription_ArgumentNullException()
        {
            _testDescription = null;

            Func<Role> act = CreateRole;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullPermissions_ArgumentNullException()
        {
            _testPermissions = null;

            Func<Role> act = CreateRole;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithInvalidCreateAtDateTime_ArgumentException(string invalidDate)
        {
            _testCreationDate = DateTime.Parse(invalidDate);

            Func<Role> act = CreateRole;

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}