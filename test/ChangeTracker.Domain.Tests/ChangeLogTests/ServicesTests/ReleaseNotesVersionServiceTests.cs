using System;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.ChangeLog.Services;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests.ServicesTests
{
    public class VersionChangeLogServiceTests
    {
        private static readonly Guid TestProjectId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
        private static readonly Guid TestVersionId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");

        [Fact]
        public void RemainingPositionsToAdd_FiveNotesExists_NinetyFiveAvailablePositions()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 5, 4);
            var service = new VersionChangeLogService(changeLogInfo);

            service.RemainingPositionsToAdd.Should().Be(95);
        }

        [Fact]
        public void NextFreePosition_LastPositionIsTen_Eleven()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 5, 10);
            var service = new VersionChangeLogService(changeLogInfo);

            service.NextFreePosition.Should().Be(11);
        }

        [Fact]
        public void AvailablePositions_MaxPositionsReached_ReturnsFalse()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 100, 112);
            var service = new VersionChangeLogService(changeLogInfo);

            service.IsPositionAvailable.Should().BeFalse();
        }

        [Fact]
        public void AvailablePositions_ThereAreFreePositions_ReturnsTrue()
        {
            var changeLogInfo = new ChangeLogInfo(TestProjectId, TestVersionId, 50, 112);
            var service = new VersionChangeLogService(changeLogInfo);

            service.IsPositionAvailable.Should().BeTrue();
        }
    }
}