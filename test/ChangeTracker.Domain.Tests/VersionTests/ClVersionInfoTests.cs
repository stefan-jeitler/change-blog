using System;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.VersionTests
{
    public class ClVersionInfoTests
    {
        private static readonly ClVersion TestVersion = ClVersion.Parse("1.2.3-dev.0");
        private static readonly Guid TestId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");
        private static readonly Guid TestProjectId = Guid.Parse("d816fb67-f2c3-4d2a-8713-f93a432fbf41");
        private static readonly DateTime TestReleaseDate = DateTime.Parse("2021-04-02T18:30");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04-02T17:30");
        private static readonly DateTime TestDeletionDate = DateTime.Parse("2021-04-02T17:31");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var version = new ClVersionInfo(TestId, TestProjectId, TestVersion, TestReleaseDate, TestCreationDate,
                null);

            version.Id.Should().Be(TestId);
            version.ProjectId.Should().Be(TestProjectId);
            version.Value.Should().Be(TestVersion);
            version.ReleasedAt.Should().Be(TestReleaseDate);
            version.CreatedAt.Should().Be(TestCreationDate);
            version.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            Func<ClVersionInfo> act = () => new ClVersionInfo(Guid.Empty,
                TestProjectId,
                TestVersion,
                TestReleaseDate,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            Func<ClVersionInfo> act = () => new ClVersionInfo(TestId,
                Guid.Empty,
                TestVersion,
                TestReleaseDate,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            Func<ClVersionInfo> act = () => new ClVersionInfo(TestId,
                TestProjectId,
                null,
                TestReleaseDate,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void Create_WithNullReleaseAtDateTime_Successful()
        {
            var version = new ClVersionInfo(TestId,
                TestProjectId, TestVersion,
                null,
                TestCreationDate,
                null);

            version.Id.Should().Be(TestId);
            version.ProjectId.Should().Be(TestProjectId);
            version.Value.Should().Be(TestVersion);
            version.ReleasedAt.HasValue.Should().BeFalse("Version is not yet released.");
            version.CreatedAt.Should().Be(TestCreationDate);
        }

        [Fact]
        public void IsReleased_WithNullReleaseAtDateTime_ReturnsFalse()
        {
            var version = new ClVersionInfo(TestId,
                TestProjectId, TestVersion,
                null,
                TestCreationDate,
                null);

            version.IsReleased.Should().BeFalse("Versions can be released later.");
        }

        [Fact]
        public void IsReleased_WithReleaseAtDateTime_ReturnsTrue()
        {
            var version = new ClVersionInfo(TestId,
                TestProjectId, TestVersion,
                DateTime.Parse("2021-04-06"),
                TestCreationDate,
                null);

            version.IsReleased.Should().BeTrue();
        }

        [Fact]
        public void IsDeleted_WithNullDeletedAtDateTime_ReturnsFalse()
        {
            var version = new ClVersionInfo(TestId,
                TestProjectId, TestVersion,
                DateTime.Parse("2021-04-06"),
                TestCreationDate,
                null);

            version.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void IsDeleted_WithReleaseAtDateTime_ReturnsTrue()
        {
            var version = new ClVersionInfo(TestId,
                TestProjectId, TestVersion,
                null,
                TestCreationDate,
                DateTime.Parse("2021-04-06"));

            version.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Create_WithReleaseAtAndDeletedAtDateTime_InvalidOperationException()
        {
            Func<ClVersionInfo> act = () => new ClVersionInfo(TestId,
                TestProjectId,
                TestVersion,
                TestReleaseDate,
                TestCreationDate,
                TestDeletionDate);

            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithReleaseAtDateTimeMinValueAndMaxValue_ArgumentException(string invalidDate)
        {
            Func<ClVersionInfo> act = () => new ClVersionInfo(TestId,
                TestProjectId,
                TestVersion,
                DateTime.Parse(invalidDate),
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithCreatedAtDateTimeMinValueAndMaxValue_ArgumentException(string invalidDate)
        {
            Func<ClVersionInfo> act = () => new ClVersionInfo(TestId,
                TestProjectId,
                TestVersion,
                TestReleaseDate,
                DateTime.Parse(invalidDate),
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithDeletedAtDateTimeMinValueAndMaxValue_ArgumentException(string invalidDate)
        {
            Func<ClVersionInfo> act = () => new ClVersionInfo(TestId,
                TestProjectId,
                TestVersion,
                TestReleaseDate,
                TestCreationDate,
                DateTime.Parse(invalidDate));

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Equals_TwoDifferentObjectsWithSameProperties_IsEqual()
        {
            var version1 = new ClVersionInfo(TestId, TestProjectId, TestVersion, TestReleaseDate, TestCreationDate,
                null);
            var version2 = new ClVersionInfo(TestId, TestProjectId, TestVersion, TestReleaseDate, TestCreationDate,
                null);

            var isEqual = version1.Equals((object) version2);

            isEqual.Should().BeTrue();
        }

        [Fact]
        public void Equals_TwoDifferentObjectsWithDifferentProperties_NotEqual()
        {
            // arrange
            var id2 = Guid.Parse("379b912d-34da-4377-8eef-dcaade0d0e09");
            var testProjectId2 = Guid.Parse("c00141a4-da01-4511-9af2-71847858424a");
            var version2 = ClVersion.Parse("5.4.3");

            var versionInfo1 = new ClVersionInfo(TestId, TestProjectId, TestVersion, TestReleaseDate, TestCreationDate,
                null);
            var versionInfo2 =
                new ClVersionInfo(id2, testProjectId2, version2, TestReleaseDate, TestCreationDate, null);

            // act
            var isEqual = versionInfo1.Equals((object) versionInfo2);

            // assert
            isEqual.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_TwoDifferentObjectsWithSameProperties_SameHashCode()
        {
            var version1 = new ClVersionInfo(TestId, TestProjectId, TestVersion, TestReleaseDate, TestCreationDate,
                null);
            var version2 = new ClVersionInfo(TestId, TestProjectId, TestVersion, TestReleaseDate, TestCreationDate,
                null);

            var hashCode1 = version1.GetHashCode();
            var hashCode2 = version2.GetHashCode();

            Assert.Equal(hashCode1, hashCode2);
        }


        [Fact]
        public void Equals_TwoDifferentObjectsWithDifferentProperties_DifferentHashCodes()
        {
            // arrange
            var id2 = Guid.Parse("379b912d-34da-4377-8eef-dcaade0d0e09");
            var testProjectId2 = Guid.Parse("c00141a4-da01-4511-9af2-71847858424a");
            var version2 = ClVersion.Parse("5.4.3");

            var versionInfo1 = new ClVersionInfo(TestId, TestProjectId, TestVersion, TestReleaseDate, TestCreationDate,
                null);
            var versionInfo2 =
                new ClVersionInfo(id2, testProjectId2, version2, TestReleaseDate, TestCreationDate, null);

            // act
            var hashCode1 = versionInfo1.GetHashCode();
            var hashCode2 = versionInfo2.GetHashCode();

            // assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}