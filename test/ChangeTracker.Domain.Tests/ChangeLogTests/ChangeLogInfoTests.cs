using System;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogInfoTests
    {
        private static readonly Guid TestProjectId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
        private static readonly Guid TestVersionId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");

        [Fact]
        public void Create_WithValidArguments_Successful()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 1, 2);

            changeLogInfo.ProjectId.Should().Be(TestProjectId);
            changeLogInfo.VersionId.Should().Be(TestVersionId);
            changeLogInfo.Count.Should().Be(1);
            changeLogInfo.LastPosition.Should().Be(2);
        }

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            Func<ChangeLogInfo> act = () => new ChangeLogInfo(Guid.Empty, TestVersionId, 1, 2);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyVersionId_ArgumentException()
        {
            Func<ChangeLogInfo> act = () => new ChangeLogInfo(TestProjectId, Guid.Empty, 1, 2);

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersionId_IsAllowed()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, null, 1, 2);

            changeLogInfo.VersionId.Should()
                .BeNull("Pending changeLog lines allowed, that are not assigned to a version yet.");
        }


        [Fact]
        public void RemainingPositionsToAdd_FiveNotesExists_NinetyFiveAvailablePositions()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 5, 4);

            changeLogInfo.RemainingPositionsToAdd.Should().Be(95);
        }

        [Fact]
        public void NextFreePosition_LastPositionIsTen_Eleven()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 5, 10);

            changeLogInfo.NextFreePosition.Should().Be(11);
        }

        [Fact]
        public void AvailablePositions_MaxPositionsReached_ReturnsFalse()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 100, 112);

            changeLogInfo.IsPositionAvailable.Should().BeFalse();
        }

        [Fact]
        public void AvailablePositions_ThereAreFreePositions_ReturnsTrue()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 50, 112);

            changeLogInfo.IsPositionAvailable.Should().BeTrue();
        }

        [Fact]
        public void LastPosition_CountIsZero_ReturnsMinusOne()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 0, 5);

            changeLogInfo.LastPosition.Should().Be(-1);
        }
        
        [Fact]
        public void LastPosition_CountIsNotZero_ReturnsPositionPassedToConstructor()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 2, 5);

            changeLogInfo.LastPosition.Should().Be(5);
        }
    }
}