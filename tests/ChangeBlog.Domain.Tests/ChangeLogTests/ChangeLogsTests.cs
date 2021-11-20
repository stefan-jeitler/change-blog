using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests.ChangeLogTests;

public class ChangeLogsTests
{
    private static Guid _testVersionId;
    private static Guid _testProductId;
    private static Guid _testUserId;

    public ChangeLogsTests()
    {
        _testVersionId = Guid.Parse("4eaa1f8e-46d4-4cdd-92d4-6a2fe6f5ac10");
        _testProductId = Guid.Parse("b43326f8-83de-44bf-83f4-b62961d71c03");
        _testUserId = Guid.Parse("294c4f04-85d4-4d5b-ae25-e6b618f1676f");
    }

    private static List<ChangeLogLine> CreateTestLines(int count)
    {
        return Enumerable
            .Range(0, count)
            .Select(x =>
                new ChangeLogLine(_testVersionId, _testProductId, ChangeLogText.Parse($"{x:D5}"), (uint)x,
                    _testUserId))
            .ToList();
    }

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
        var changeLogs = new ChangeLogs(CreateTestLines(1));

        changeLogs.VersionId.Should().HaveValue();
        changeLogs.VersionId!.Value.Should().Be(_testVersionId);
    }

    [Fact]
    public void Create_WithLinesFromDifferentVersions_InvalidOperationException()
    {
        // arrange
        var differentVersion = Guid.Parse("267fd0d6-678c-4796-b354-27d82a15687b");
        var lineFromDifferentVersion =
            new ChangeLogLine(differentVersion, _testProductId, ChangeLogText.Parse("some text"), 1, _testUserId);

        var testLines = CreateTestLines(1);
        testLines.Add(lineFromDifferentVersion);

        // act
        var act = () => new ChangeLogs(testLines);

        // assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Create_WithOnePendingLineAndOneAssignedLine_InvalidOperationException()
    {
        // arrange
        var pendingLine =
            new ChangeLogLine(null, _testProductId, ChangeLogText.Parse("some text"), 1, _testUserId);

        var testLines = CreateTestLines(1);
        testLines.Add(pendingLine);

        // act
        var act = () => new ChangeLogs(testLines);

        // assert
        act.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void Create_With10Lines_90AvailablePositionsToAdd()
    {
        // arrange
        var testLines = CreateTestLines(10);

        // act
        var changeLogs = new ChangeLogs(testLines);

        // assert
        changeLogs.RemainingPositionsToAdd.Should().Be(90);
    }

    [Fact]
    public void Create_With10Lines_NextFreePositionIsTen()
    {
        // arrange
        var testLines = CreateTestLines(10);

        // act
        var changeLogs = new ChangeLogs(testLines);

        // assert
        changeLogs.NextFreePosition.Should().Be(10);
    }

    [Fact]
    public void Create_WithHighPosition123_NextFreePositionIs124()
    {
        // arrange
        var lineWithHighPosition = new ChangeLogLine(_testVersionId,
            _testProductId, ChangeLogText.Parse("some text"), 124, _testUserId);

        var testLines = CreateTestLines(10);
        testLines.Add(lineWithHighPosition);

        // act
        var changeLogs = new ChangeLogs(testLines);

        // assert
        changeLogs.NextFreePosition.Should().Be(125);
    }

    [Fact]
    public void Create_WithTenLines_CountIsTen()
    {
        // arrange
        var testLines = CreateTestLines(10);

        // act
        var changeLogs = new ChangeLogs(testLines);

        // assert
        changeLogs.Count.Should().Be(10);
    }

    [Fact]
    public void Create_With100Lines_NotRemainingPositionsToAdd()
    {
        // arrange
        var testLines = CreateTestLines(100);

        // act
        var changeLogs = new ChangeLogs(testLines);

        // assert
        changeLogs.IsPositionAvailable.Should().BeFalse();
    }

    [Fact]
    public void Create_With90Lines_PositionIsAvailable()
    {
        // arrange
        var testLines = CreateTestLines(90);

        // act
        var changeLogs = new ChangeLogs(testLines);

        // assert
        changeLogs.IsPositionAvailable.Should().BeTrue();
    }

    [Fact]
    public void Create_WithDuplicateTexts_ArgumentException()
    {
        // arrange
        var line1 = CreateTestLines(1);
        var line2 = CreateTestLines(1);
        var lines = line1.Concat(line2).ToList();

        // act
        var act = () => new ChangeLogs(lines);

        // assert
        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithTenLines_TenItemsExists()
    {
        var lines = CreateTestLines(10);

        var changeLogs = new ChangeLogs(lines);

        changeLogs.Count.Should().Be(10);
    }

    [Fact]
    public void ContainsText_SameTextExists_ReturnsTrue()
    {
        var lines = CreateTestLines(2);
        var changeLogs = new ChangeLogs(lines);

        var containsText = changeLogs.ContainsText(ChangeLogText.Parse("00000"));

        containsText.Should().BeTrue();
    }

    [Fact]
    public void ContainsText_SameTextDifferentCase_ReturnsTrue()
    {
        var line = new ChangeLogLine(_testVersionId, _testProductId, ChangeLogText.Parse("new Feature"), 0,
            _testUserId);
        var changeLogs = new ChangeLogs(new List<ChangeLogLine>(1) { line });

        var containsText = changeLogs.ContainsText(ChangeLogText.Parse("New Feature"));

        containsText.Should().BeTrue();
    }

    [Fact]
    public void ContainsText_DifferentText_ReturnsFalse()
    {
        var line = new ChangeLogLine(_testVersionId, _testProductId, ChangeLogText.Parse("new Feature"), 0,
            _testUserId);
        var changeLogs = new ChangeLogs(new List<ChangeLogLine>(1) { line });

        var containsText = changeLogs.ContainsText(ChangeLogText.Parse("Bugfix"));

        containsText.Should().BeFalse();
    }

    [Fact]
    public void FindDuplicateTexts_NoDuplicatesExists_ReturnsEmptySequence()
    {
        // arrange
        var lines = new List<ChangeLogLine>
        {
            new(_testVersionId, _testProductId, ChangeLogText.Parse("new Feature"), 0, _testUserId),
            new(_testVersionId, _testProductId, ChangeLogText.Parse("Bugfix"), 1, _testUserId)
        };
        var changeLogs = new ChangeLogs(lines);

        var otherLines = new List<ChangeLogLine>
        {
            new(_testVersionId, _testProductId, ChangeLogText.Parse("feature added"), 0, _testUserId),
            new(_testVersionId, _testProductId, ChangeLogText.Parse("Security fix"), 1, _testUserId)
        };

        // act
        var duplicates = changeLogs.FindDuplicateTexts(otherLines);

        // assert
        duplicates.Should().BeEmpty();
    }

    [Fact]
    public void FindDuplicateTexts_DuplicatesWithDifferentCasingExists_ReturnsDuplicate()
    {
        // arrange
        var lines = new List<ChangeLogLine>
        {
            new(_testVersionId, _testProductId, ChangeLogText.Parse("new Feature"), 0, _testUserId),
            new(_testVersionId, _testProductId, ChangeLogText.Parse("Bugfix"), 1, _testUserId)
        };
        var changeLogs = new ChangeLogs(lines);

        var otherLines = new List<ChangeLogLine>
        {
            new(_testVersionId, _testProductId, ChangeLogText.Parse("NEW feature"), 0, _testUserId),
            new(_testVersionId, _testProductId, ChangeLogText.Parse("Security fix"), 1, _testUserId)
        };

        // act
        var duplicates = changeLogs.FindDuplicateTexts(otherLines).ToList();

        // assert
        duplicates.Should().HaveCount(1);
        duplicates.Single().Text.Value.Should().Be("new Feature");
    }

    [Fact]
    public void FindDuplicateTexts_DuplicatesWithSameCasingExists_ReturnsDuplicate()
    {
        // arrange
        var lines = new List<ChangeLogLine>
        {
            new(_testVersionId, _testProductId, ChangeLogText.Parse("new Feature"), 0, _testUserId),
            new(_testVersionId, _testProductId, ChangeLogText.Parse("Bugfix"), 1, _testUserId)
        };
        var changeLogs = new ChangeLogs(lines);

        var otherLines = new List<ChangeLogLine>
        {
            new(_testVersionId, _testProductId, ChangeLogText.Parse("new Feature"), 0, _testUserId),
            new(_testVersionId, _testProductId, ChangeLogText.Parse("Security fix"), 1, _testUserId)
        };

        // act
        var duplicates = changeLogs.FindDuplicateTexts(otherLines).ToList();

        // assert
        duplicates.Should().HaveCount(1);
        duplicates.Single().Text.Value.Should().Be("new Feature");
    }
}