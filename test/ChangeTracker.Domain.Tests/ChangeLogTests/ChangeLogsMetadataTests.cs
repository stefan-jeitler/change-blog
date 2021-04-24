using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests
{
    public class ChangeLogsMetadataTests
    {
        private static readonly Guid TestProjectId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
        private static readonly Guid TestVersionId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");

        private static List<ChangeLogLine> CreateTestLines(int count) => Enumerable
            .Range(0, count)
            .Select(x => new ChangeLogLine(TestVersionId, TestProjectId, ChangeLogText.Parse($"{x:D5}"), (uint) x))
            .ToList();

        [Fact]
        public void Create_WithEmptyLines_ProjectIdIsEmpty()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, Array.Empty<ChangeLogLine>());

            metadata.ProjectId.Should().Be(TestProjectId);
        }

        [Fact]
        public void Create_WithEmptyLines_VersionIsNull()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, Array.Empty<ChangeLogLine>());

            metadata.VersionId.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyLines_CountIsZero()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, Array.Empty<ChangeLogLine>());

            metadata.Count.Should().Be(0);
        }

        [Fact]
        public void Create_WithEmptyLines_LastPositionIsMinusOne()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, Array.Empty<ChangeLogLine>());

            metadata.LastPosition.Should().Be(-1);
        }

        [Fact]
        public void Create_WithEmptyLines_NextFreePositionIsZero()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, Array.Empty<ChangeLogLine>());

            metadata.NextFreePosition.Should().Be(0);
        }

        [Fact]
        public void Create_WithEmptyLines_PositionsAvailable()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, Array.Empty<ChangeLogLine>());

            metadata.IsPositionAvailable.Should().Be(true);
        }

        [Fact]
        public void Create_WithEmptyLines_MaxChangeLogLinesToAdd()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, Array.Empty<ChangeLogLine>());

            metadata.RemainingPositionsToAdd.Should().Be(ChangeLogsMetadata.MaxChangeLogLines);
        }

        [Fact]
        public void Create_WithTestLines_ProjectIdAndVersionIdProperlyAssigned()
        {
            var metadata = ChangeLogsMetadata.Create(TestProjectId, CreateTestLines(1));

            metadata.ProjectId.Should().Be(TestProjectId);
            metadata.VersionId.Should().HaveValue();
            metadata.VersionId!.Value.Should().Be(TestVersionId);
        }

        [Fact]
        public void Create_WithLinesFromDifferentProjects_InvalidOperationException()
        {
            // arrange
            var anotherProjectId = Guid.Parse("df8b6258-f160-40f0-a72d-fd7aa3601bc2");
            var lineFromDifferentProject =
                new ChangeLogLine(TestVersionId, anotherProjectId, ChangeLogText.Parse("some text"), 1);

            var testLines = CreateTestLines(1);
            testLines.Add(lineFromDifferentProject);

            // act
            Func<ChangeLogsMetadata> act = () => ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Create_WithDifferentProjectIdArgument_ArgumentException()
        {
            // arrange
            var anotherProjectId = Guid.Parse("df8b6258-f160-40f0-a72d-fd7aa3601bc2");

            var testLines = CreateTestLines(1);

            // act
            Func<ChangeLogsMetadata> act = () => ChangeLogsMetadata.Create(anotherProjectId, testLines);

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithDifferentProjectIdArgumentAndDifferentProjectIdsInLines_InvalidOperationException()
        {
            // arrange
            var anotherProjectId = Guid.Parse("df8b6258-f160-40f0-a72d-fd7aa3601bc2");
            var yetAnotherProjectId = Guid.Parse("c2196b5e-e829-402b-b1af-a28c5b87b8cc");
            var line = new ChangeLogLine(TestVersionId, yetAnotherProjectId, ChangeLogText.Parse("test text"), 1);
            var testLines = CreateTestLines(1);
            testLines.Add(line);

            // act
            Func<ChangeLogsMetadata> act = () => ChangeLogsMetadata.Create(anotherProjectId, testLines);

            // assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Create_WithLinesOfDifferentVersions_InvalidOperationException()
        {
            // arrange
            var differentVersion = Guid.Parse("267fd0d6-678c-4796-b354-27d82a15687b");
            var lineFromDifferentVersion =
                new ChangeLogLine(differentVersion, TestProjectId, ChangeLogText.Parse("some text"), 1);

            var testLines = CreateTestLines(1);
            testLines.Add(lineFromDifferentVersion);

            // act
            Func<ChangeLogsMetadata> act = () => ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Create_WithOnePendingLineAndOneAssignedLine_InvalidOperationException()
        {
            // arrange
            var pendingLine =
                new ChangeLogLine(null, TestProjectId, ChangeLogText.Parse("some text"), 1);

            var testLines = CreateTestLines(1);
            testLines.Add(pendingLine);

            // act
            Func<ChangeLogsMetadata> act = () => ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Fact]
        public void Create_With10Lines_90AvailablePositionsToAdd()
        {
            // arrange
            var testLines = CreateTestLines(10);

            // act
            var metadata = ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            metadata.RemainingPositionsToAdd.Should().Be(90);
        }

        [Fact]
        public void Create_With10Lines_NextFreePositionIsTen()
        {
            // arrange
            var testLines = CreateTestLines(10);

            // act
            var metadata = ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            metadata.NextFreePosition.Should().Be(10);
        }

        [Fact]
        public void Create_WithHighPosition123_NextFreePositionIs124()
        {
            // arrange
            var lineWithHighPosition = new ChangeLogLine(TestVersionId,
                TestProjectId, ChangeLogText.Parse("some text"), 124);

            var testLines = CreateTestLines(10);
            testLines.Add(lineWithHighPosition);

            // act
            var metadata = ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            metadata.NextFreePosition.Should().Be(125);
        }

        [Fact]
        public void Create_WithTenLines_CountIsTen()
        {
            // arrange
            var testLines = CreateTestLines(10);

            // act
            var metadata = ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            metadata.Count.Should().Be(10);
        }

        [Fact]
        public void Create_With100Lines_NotRemainingPositionsToAdd()
        {
            // arrange
            var testLines = CreateTestLines(100);

            // act
            var metadata = ChangeLogsMetadata.Create(TestProjectId, testLines);

            // assert
            metadata.IsPositionAvailable.Should().BeFalse();
        }

        [Fact]
        public void Create_With90Lines_PositionIsAvailable()
        {
            // arrange
            var testLines = CreateTestLines(90);

            // act
            var metadata = ChangeLogsMetadata.Create(TestProjectId, testLines);

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
            Func<ChangeLogsMetadata> act = () => ChangeLogsMetadata.Create(TestProjectId, lines);

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithTenLines_TenTexts()
        {
            var lines = CreateTestLines(10);

            var metadata = ChangeLogsMetadata.Create(TestProjectId, lines);

            metadata.Texts.Count.Should().Be(10);
        }
    }
}