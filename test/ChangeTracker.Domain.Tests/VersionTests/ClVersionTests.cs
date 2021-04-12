using System;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.VersionTests
{
    public class ClVersionTests
    {
        private static readonly ClVersionValue TestVersionValue = ClVersionValue.Parse("1.2.3-dev.0");
        private static readonly Guid TestId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");
        private static readonly Guid TestProjectId = Guid.Parse("d816fb67-f2c3-4d2a-8713-f93a432fbf41");
        private static readonly DateTime TestReleaseDate =  DateTime.Parse("2021-04-02T19:30");
        private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04-02T17:30");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var version = new ClVersion(TestId, TestProjectId, TestVersionValue, TestReleaseDate, TestCreationDate,
                null);

            version.Id.Should().Be(TestId);
            version.ProjectId.Should().Be(TestProjectId);
            version.Value.Should().Be(TestVersionValue);
            version.ReleasedAt.Should().Be(TestReleaseDate);
            version.CreatedAt.Should().Be(TestCreationDate);
            version.DeletedAt.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyId_ArgumentException()
        {
            Func<ClVersion> act = () => new ClVersion(Guid.Empty,
                TestProjectId,
                TestVersionValue,
                TestReleaseDate,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            Func<ClVersion> act = () => new ClVersion(TestId,
                Guid.Empty,
                TestVersionValue,
                TestReleaseDate,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            Func<ClVersion> act = () => new ClVersion(TestId,
                TestProjectId,
                null,
                TestReleaseDate,
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void IsReleased_ReleaseAtIsNull_ReturnsFalse()
        {
            var version = new ClVersion(TestId,
                TestProjectId, TestVersionValue,
                null,
                TestCreationDate,
                null);

            version.IsReleased.Should().BeFalse("Versions can be released later.");
        }

        [Fact]
        public void IsReleased_ReleaseAtIsNotNull_ReturnsTrue()
        {
            var version = new ClVersion(TestId,
                TestProjectId, TestVersionValue,
                DateTime.Parse("2021-04-06"),
                TestCreationDate,
                null);

            version.IsReleased.Should().BeTrue();
        }

        [Fact]
        public void IsDeleted_DeletedAtIsNull_ReturnsFalse()
        {
            var version = new ClVersion(TestId,
                TestProjectId, TestVersionValue,
                DateTime.Parse("2021-04-06"),
                TestCreationDate,
                null);

            version.IsDeleted.Should().BeFalse();
        }

        [Fact]
        public void IsDeleted_DeletedAtIsNotNull_ReturnsTrue()
        {
            var version = new ClVersion(TestId,
                TestProjectId, TestVersionValue,
                null,
                TestCreationDate,
                DateTime.Parse("2021-04-06"));

            version.IsDeleted.Should().BeTrue();
        }

        [Fact]
        public void Create_ReleaseAtAfterDeletedAt_InvalidOperationException()
        {
            Func<ClVersion> act = () => new ClVersion(TestId,
                TestProjectId,
                TestVersionValue,
                DateTime.Parse("2021-04-12T19:30:00"),
                TestCreationDate,
                DateTime.Parse("2021-04-12T18:30:00"));

            act.Should().ThrowExactly<InvalidOperationException>("Deleted version cannot be released.");
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithReleasedAtTimeMinAndMaxValue_ArgumentException(string invalidDate)
        {
            Func<ClVersion> act = () => new ClVersion(TestId,
                TestProjectId,
                TestVersionValue,
                DateTime.Parse(invalidDate),
                TestCreationDate,
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithCreatedAtTimeMinAndMaxValue_ArgumentException(string invalidDate)
        {
            Func<ClVersion> act = () => new ClVersion(TestId,
                TestProjectId,
                TestVersionValue,
                TestReleaseDate,
                DateTime.Parse(invalidDate),
                null);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Theory]
        [InlineData("0001-01-01T00:00:00.0000000")]
        [InlineData("9999-12-31T23:59:59.9999999")]
        public void Create_WithDeletedAtTimeMinAndMaxValue_ArgumentException(string invalidDate)
        {
            Func<ClVersion> act = () => new ClVersion(TestId,
                TestProjectId,
                TestVersionValue,
                TestReleaseDate,
                TestCreationDate,
                DateTime.Parse(invalidDate));

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Equals_TwoDifferentObjectsWithSameProperties_IsEqual()
        {
            var version1 = new ClVersion(TestId, TestProjectId, TestVersionValue, TestReleaseDate, TestCreationDate,
                null);
            var version2 = new ClVersion(TestId, TestProjectId, TestVersionValue, TestReleaseDate, TestCreationDate,
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
            var version2 = ClVersionValue.Parse("5.4.3");

            var versionInfo1 = new ClVersion(TestId, TestProjectId, TestVersionValue, TestReleaseDate, TestCreationDate,
                null);
            var versionInfo2 =
                new ClVersion(id2, testProjectId2, version2, TestReleaseDate, TestCreationDate, null);

            // act
            var isEqual = versionInfo1.Equals((object) versionInfo2);

            // assert
            isEqual.Should().BeFalse();
        }

        [Fact]
        public void GetHashCode_TwoDifferentObjectsWithSameProperties_SameHashCode()
        {
            var version1 = new ClVersion(TestId, TestProjectId, TestVersionValue, TestReleaseDate, TestCreationDate,
                null);
            var version2 = new ClVersion(TestId, TestProjectId, TestVersionValue, TestReleaseDate, TestCreationDate,
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
            var version2 = ClVersionValue.Parse("5.4.3");

            var versionInfo1 = new ClVersion(TestId, TestProjectId, TestVersionValue, TestReleaseDate, TestCreationDate,
                null);
            var versionInfo2 =
                new ClVersion(id2, testProjectId2, version2, TestReleaseDate, TestCreationDate, null);

            // act
            var hashCode1 = versionInfo1.GetHashCode();
            var hashCode2 = versionInfo2.GetHashCode();

            // assert
            Assert.NotEqual(hashCode1, hashCode2);
        }
    }
}