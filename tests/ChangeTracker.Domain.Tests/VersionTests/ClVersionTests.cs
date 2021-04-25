using System;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.VersionTests
{
    public class ClVersionTests
    {
        private DateTime _testCreationDate;
        private DateTime? _testDeletedDate;
        private Guid _testId;
        private Guid _testProjectId;
        private DateTime? _testReleaseDate;
        private ClVersionValue _testVersionValue;

        public ClVersionTests()
        {
            _testVersionValue = ClVersionValue.Parse("1.2.3-dev.0");
            _testId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");
            _testProjectId = Guid.Parse("d816fb67-f2c3-4d2a-8713-f93a432fbf41");
            _testReleaseDate = DateTime.Parse("2021-04-02T19:30");
            _testCreationDate = DateTime.Parse("2021-04-02T17:30");
            _testDeletedDate = null;
        }

        private ClVersion CreateVersion() => new(_testId, _testProjectId, _testVersionValue, _testReleaseDate,
            _testCreationDate, _testDeletedDate);

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var version = CreateVersion();

            version.Id.Should().Be(_testId);
            version.ProjectId.Should().Be(_testProjectId);
            version.Value.Should().Be(_testVersionValue);
            version.ReleasedAt.Should().Be(_testReleaseDate);
            version.CreatedAt.Should().Be(_testCreationDate);
            version.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            _testId = Guid.Empty;

            Func<ClVersion> act = CreateVersion;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            _testProjectId = Guid.Empty;

            Func<ClVersion> act = CreateVersion;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            _testVersionValue = null;

            Func<ClVersion> act = CreateVersion;

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void IsReleased_ReleaseAtIsNull_ReturnsFalse()
        {
            _testReleaseDate = null;

            var version = CreateVersion();

            version.IsReleased.Should().BeFalse("Versions can be released later.");
        }

        [Fact]
        public void IsReleased_ReleaseAtIsNotNull_ReturnsTrue()
        {
            _testReleaseDate = DateTime.Parse("2021-04-16");

            var version = CreateVersion();

            version.IsReleased.Should().BeTrue();
        }

        [Fact]
        public void IsClosed_DeletedAtIsNull_ReturnsFalse()
        {
            _testDeletedDate = null;

            var version = CreateVersion();

            version.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void IsClosed_DeletedAtIsNotNull_ReturnsTrue()
        {
            _testDeletedDate = DateTime.Parse("2021-04-16");

            var version = CreateVersion();

            version.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Create_ReleasedAtAfterDeletedDate_InvalidOperationException()
        {
            _testDeletedDate = DateTime.Parse("2021-04-16T18:30:00Z");
            _testReleaseDate = DateTime.Parse("2021-04-16T18:30:01Z");

            Func<ClVersion> act = CreateVersion;

            act.Should().ThrowExactly<InvalidOperationException>("Closed versions cannot be released.");
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithReleasedAtTimeMinAndMaxValue_ArgumentException(string invalidDate)
        {
            _testReleaseDate = DateTime.Parse(invalidDate);

            Func<ClVersion> act = CreateVersion;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithCreatedAtTimeMinAndMaxValue_ArgumentException(string invalidDate)
        {
            _testCreationDate = DateTime.Parse(invalidDate);

            Func<ClVersion> act = CreateVersion;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithDeletedAtTimeMinAndMaxValue_ArgumentException(string invalidDate)
        {
            _testDeletedDate = DateTime.Parse(invalidDate);

            Func<ClVersion> act = CreateVersion;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Equals_TwoDifferentObjectsWithSameProperties_IsEqual()
        {
            var version1 = CreateVersion();
            var version2 = CreateVersion();

            var isEqual = version1.Equals((object) version2);

            isEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoDifferentObjectsWithDifferentProperties_NotEqual()
        {
            // arrange
            var version1 = CreateVersion();

            _testId = Guid.Parse("379b912d-34da-4377-8eef-dcaade0d0e09");
            _testProjectId = Guid.Parse("c00141a4-da01-4511-9af2-71847858424a");
            var version2 = CreateVersion();

            // act
            var isEqual = version1.Equals((object) version2);

            // assert
            isEqual.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_TwoDifferentObjectsWithSameProperties_SameHashCode()
        {
            var version1 = CreateVersion();
            var version2 = CreateVersion();

            var hashCode1 = version1.GetHashCode();
            var hashCode2 = version2.GetHashCode();

            Assert.Equal(hashCode1, hashCode2);
        }


        [Fact]
        public void Equals_TwoDifferentObjectsWithDifferentProperties_DifferentHashCodes()
        {
            // arrange
            var version1 = CreateVersion();

            _testId = Guid.Parse("379b912d-34da-4377-8eef-dcaade0d0e09");
            _testProjectId = Guid.Parse("c00141a4-da01-4511-9af2-71847858424a");
            var version2 = CreateVersion();

            // act
            var hashCode1 = version1.GetHashCode();
            var hashCode2 = version2.GetHashCode();

            // assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}