using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogsTests
    {
        private static Guid _testVersionId;
        private static Guid _testProjectId;

        public ChangeLogsTests()
        {
            _testVersionId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");
            _testProjectId = Guid.Parse("b43326f8-83de-44bf-83f4-b62961d71c03");
        }

        private static List<ChangeLogLine> CreateTestLines(int count) => Enumerable
            .Range(0, count)
            .Select(x => new ChangeLogLine(_testVersionId, _testProjectId, ChangeLogText.Parse($"{x:D5}"), (uint) x))
            .ToList();

        [Fact]
        public void Create_WithEmptyLines_VersionIsNull()
        {
            var changeLogs = new ChangeLogs(Array.Empty<ChangeLogLine>());

            changeLogs.VersionId.Should().NotHaveValue();
        }

        [Fact]
        public void Create_WithEmptyLines_CountIsZero()
        {
            var changeLogs = new ChangeLogs(Array.Empty<ChangeLogLine>());

            changeLogs.Count.Should().Be(0);
        }

        [Fact]
        public void Create_WithEmptyLines_LastPositionIsMinusOne()
        {
            var changeLogs = new ChangeLogs(Array.Empty<ChangeLogLine>());

            changeLogs.LastPosition.Should().Be(-1);
        }

        [Fact]
        public void Create_WithEmptyLines_NextFreePositionIsZero()
        {
            var changeLogs = new ChangeLogs(Array.Empty<ChangeLogLine>());

            changeLogs.NextFreePosition.Should().Be(0);
        }

        [Fact]
        public void Create_WithEmptyLines_PositionsAvailable()
        {
            var changeLogs = new ChangeLogs(Array.Empty<ChangeLogLine>());

            changeLogs.IsPositionAvailable.Should().Be(true);
        }

        [Fact]
        public void Create_WithEmptyLines_MaxChangeLogLinesToAdd()
        {
            var changeLogs = new ChangeLogs(Array.Empty<ChangeLogLine>());

            changeLogs.RemainingPositionsToAdd.Should().Be(ChangeLogs.MaxLines);
        }

        [Fact]
        public void Create_WithTestLines_VersionIdProperlyAssigned()
        {
            var metadata = new ChangeLogs(CreateTestLines(1));

            metadata.VersionId.Should().HaveValue();
            metadata.VersionId!.Value.Should().Be(_testVersionId);
        }

        [Fact]
        public void Create_WithLinesFromDifferentVersions_InvalidOperationException()
        {
            // arrange
            var differentVersion = Guid.Parse("267fd0d6-678c-4796-b354-27d82a15687b");
            var lineFromDifferentVersion =
                new ChangeLogLine(differentVersion, _testProjectId, ChangeLogText.Parse("some text"), 1);

            var testLines = CreateTestLines(1);
            testLines.Add(lineFromDifferentVersion);

            // act
            Func<ChangeLogs> act = () => new ChangeLogs(testLines);

            // assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Create_WithOnePendingLineAndOneAssignedLine_InvalidOperationException()
        {
            // arrange
            var pendingLine =
                new ChangeLogLine(null, _testProjectId, ChangeLogText.Parse("some text"), 1);

            var testLines = CreateTestLines(1);
            testLines.Add(pendingLine);

            // act
            Func<ChangeLogs> act = () => new ChangeLogs(testLines);

            // assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Create_With10Lines_90AvailablePositionsToAdd()
        {
            // arrange
            var testLines = CreateTestLines(10);

            // act
            var metadata = new ChangeLogs(testLines);

            // assert
            metadata.RemainingPositionsToAdd.Should().Be(90);
        }

        [Fact]
        public void Create_With10Lines_NextFreePositionIsTen()
        {
            // arrange
            var testLines = CreateTestLines(10);

            // act
            var metadata = new ChangeLogs(testLines);

            // assert
            metadata.NextFreePosition.Should().Be(10);
        }

        [Fact]
        public void Create_WithHighPosition123_NextFreePositionIs124()
        {
            // arrange
            var lineWithHighPosition = new ChangeLogLine(_testVersionId,
                _testProjectId, ChangeLogText.Parse("some text"), 124);

            var testLines = CreateTestLines(10);
            testLines.Add(lineWithHighPosition);

            // act
            var metadata = new ChangeLogs(testLines);

            // assert
            metadata.NextFreePosition.Should().Be(125);
        }

        [Fact]
        public void Create_WithTenLines_CountIsTen()
        {
            // arrange
            var testLines = CreateTestLines(10);

            // act
            var metadata = new ChangeLogs(testLines);

            // assert
            metadata.Count.Should().Be(10);
        }

        [Fact]
        public void Create_With100Lines_NotRemainingPositionsToAdd()
        {
            // arrange
            var testLines = CreateTestLines(100);

            // act
            var metadata = new ChangeLogs(testLines);

            // assert
            metadata.IsPositionAvailable.Should().BeFalse();
        }

        [Fact]
        public void Create_With90Lines_PositionIsAvailable()
        {
            // arrange
            var testLines = CreateTestLines(90);

            // act
            var metadata = new ChangeLogs(testLines);

            // assert
            metadata.IsPositionAvailable.Should().BeTrue();
        }

        [Fact]
        public void Create_WithDuplicateTexts_ArgumentException()
        {
            // arrange
            var line1 = CreateTestLines(1);
            var line2 = CreateTestLines(1);
            var lines = line1.Concat(line2).ToList();

            // act
            Func<ChangeLogs> act = () => new ChangeLogs(lines);

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithTenLines_TenItemsExists()
        {
            var lines = CreateTestLines(10);

            var metadata = new ChangeLogs(lines);

            metadata.Count.Should().Be(10);
        }
    }
}