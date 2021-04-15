using System;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogsMetadataTests
    {
        private static readonly Guid TestProjectId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
        private static readonly Guid TestVersionId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, TestVersionId, 1, 2);

            changeLogsMetaData.ProjectId.Should().Be(TestProjectId);
            changeLogsMetaData.VersionId.Should().Be(TestVersionId);
            changeLogsMetaData.Count.Should().Be(1);
            changeLogsMetaData.LastPosition.Should().Be(2);
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            Func<ChangeLogsMetadata> act = () => new ChangeLogsMetadata(Guid.Empty, TestVersionId, 1, 2);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyVersionId_ArgumentException()
        {
            Func<ChangeLogsMetadata> act = () => new ChangeLogsMetadata(TestProjectId, Guid.Empty, 1, 2);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersionId_IsAllowed()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, null, 1, 2);

            changeLogsMetaData.VersionId.Should()
                .BeNull("Pending changeLog lines allowed, that are not assigned to a version yet.");
        }

        [Fact]
        public void RemainingPositionsToAdd_FiveNotesExists_NinetyFiveAvailablePositions()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, TestVersionId, 5, 4);

            changeLogsMetaData.RemainingPositionsToAdd.Should().Be(95);
        }

        [Fact]
        public void NextFreePosition_LastPositionIsTen_ReturnsEleven()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, TestVersionId, 5, 10);

            changeLogsMetaData.NextFreePosition.Should().Be(11);
        }

        [Fact]
        public void AvailablePositions_MaxPositionsReached_ReturnsFalse()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, TestVersionId, 100, 112);

            changeLogsMetaData.IsPositionAvailable.Should().BeFalse();
        }

        [Fact]
        public void AvailablePositions_ThereAreFreePositions_ReturnsTrue()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, TestVersionId, 50, 112);

            changeLogsMetaData.IsPositionAvailable.Should().BeTrue();
        }

        [Fact]
        public void LastPosition_CountIsZero_ReturnsMinusOne()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, TestVersionId, 0, 5);

            changeLogsMetaData.LastPosition.Should().Be(-1);
        }

        [Fact]
        public void LastPosition_CountIsNotZero_ReturnsPositionPassedToConstructor()
        {
            var changeLogsMetaData = new ChangeLogsMetadata(TestProjectId, TestVersionId, 1, 5);

            changeLogsMetaData.LastPosition.Should().Be(5);
        }
    }
}