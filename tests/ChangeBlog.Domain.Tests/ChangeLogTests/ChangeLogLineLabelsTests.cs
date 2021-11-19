using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeBlog.Domain.Tests.ChangeLogTests;

public class ChangeLogLineLabelsTests
{
    private const uint TestPosition = 5;
    private static readonly Guid TestId = Guid.Parse("51d89265-52c2-4a38-a0fe-b99bdc5523d0");
    private static readonly Guid TestVersionId = Guid.Parse("66845d0a-45bc-4834-96d0-b48c2c403628");
    private static readonly Guid TestProductId = Guid.Parse("ef5656e5-15f0-418d-b3a4-b69f1c3abac5");
    private static readonly ChangeLogText TestText = ChangeLogText.Parse("New feature added");
    private static readonly Guid TestUserId = Guid.Parse("294c4f04-85d4-4d5b-ae25-e6b618f1676f");
    private static readonly DateTime TestCreationDate = DateTime.Parse("2021-04.02T18:28");

    [Fact]
    public void Create_WithLabels_LabelsExists()
    {
        // arrange
        var featureLabel = Label.Parse("Feature");
        var bugLabel = Label.Parse("Bug");
        var labels = new List<Label>
        {
            featureLabel,
            bugLabel
        };

        // act
        var line = new ChangeLogLine(TestId, TestVersionId,
            TestProductId, TestText, TestPosition,
            TestCreationDate, labels, Array.Empty<Issue>(), TestUserId);

        // assert
        line.Labels.Count.Should().Be(2);
        line.Labels.Should().Contain(featureLabel);
        line.Labels.Should().Contain(bugLabel);
    }

    [Fact]
    public void Create_WithNullLabels_ArgumentNullException()
    {
        Func<ChangeLogLine> act = () => new ChangeLogLine(TestId,
            TestVersionId,
            TestProductId,
            TestText,
            TestPosition,
            TestCreationDate,
            null,
            Enumerable.Empty<Issue>(),
            TestUserId);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithTooManyLabels_ArgumentException()
    {
        var labels = new[]
        {
            Label.Parse("Added"), Label.Parse("Changed"),
            Label.Parse("Deprecated"), Label.Parse("Removed"),
            Label.Parse("Fixed"), Label.Parse("Security")
        };

        Func<ChangeLogLine> act = () => new ChangeLogLine(TestId, TestVersionId,
            TestProductId, TestText, TestPosition,
            TestCreationDate, labels, Array.Empty<Issue>(),
            TestUserId);

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithoutLabels_Empty()
    {
        var line = new ChangeLogLine(TestId, TestVersionId,
            TestProductId, TestText, TestPosition,
            TestUserId, TestCreationDate);

        line.Labels.Should().BeEmpty();
    }

    [Fact]
    public void AddLabel_ToLine_LabelAdded()
    {
        var featureLabel = Label.Parse("Feature");
        var line = new ChangeLogLine(TestId, null,
            TestProductId, TestText, TestPosition,
            TestUserId, TestCreationDate);

        line.AddLabel(featureLabel);

        line.Labels.Should().Contain(featureLabel);
    }

    [Fact]
    public void AddLabel_MaxLabelsReached_ArgumentException()
    {
        // arrange
        var labels = new List<Label>
        {
            Label.Parse("Added"), Label.Parse("Changed"),
            Label.Parse("Deprecated"), Label.Parse("Removed"),
            Label.Parse("Fixed")
        };

        var line = new ChangeLogLine(TestId, null,
            TestProductId, TestText, TestPosition,
            TestCreationDate, labels, Array.Empty<Issue>(),
            TestUserId);

        var newLabel = Label.Parse("Security");

        // act
        Action act = () => line.AddLabel(newLabel);

        // assert
        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void AddLabel_ToExistingLabels_NewLabelAdded()
    {
        // arrange
        var featureLabel = Label.Parse("Feature");
        var bugLabel = Label.Parse("Bug");
        var deprecatedLabel = Label.Parse("Deprecated");
        var existingLabels = new List<Label> {featureLabel, bugLabel};

        var line = new ChangeLogLine(TestId, null,
            TestProductId, TestText, TestPosition,
            TestCreationDate, existingLabels, Array.Empty<Issue>(),
            TestUserId);

        // act
        line.AddLabel(deprecatedLabel);

        // assert
        line.Labels.Count.Should().Be(3);
        line.Labels.Should().Contain(featureLabel);
        line.Labels.Should().Contain(bugLabel);
        line.Labels.Should().Contain(deprecatedLabel);
    }

    [Fact]
    public void Create_WithSameLabels_NoDuplicates()
    {
        // arrange
        var featureLabel = Label.Parse("Feature");
        var existingLabels = new List<Label> {featureLabel, featureLabel};

        // act
        var line = new ChangeLogLine(TestId, TestVersionId,
            TestProductId, TestText, TestPosition,
            TestCreationDate, existingLabels, Array.Empty<Issue>(), TestUserId);

        // assert
        line.Labels.Count.Should().Be(1);
        line.Labels.First().Should().Be(featureLabel);
    }

    [Fact]
    public void AvailableLabelPlaces_EmptyLabels_ReturnsMaxLabels()
    {
        // arrange
        var line = new ChangeLogLine(TestId, TestVersionId,
            TestProductId, TestText, TestPosition,
            TestCreationDate, Enumerable.Empty<Label>(), Enumerable.Empty<Issue>(), TestUserId);

        // act
        var remainingLabelPlaces = line.AvailableLabelPlaces;

        // assert
        remainingLabelPlaces.Should().Be(ChangeLogLine.MaxLabels);
    }

    [Fact]
    public void AvailableLabelPlaces_OneLabelExists_ReturnsFour()
    {
        // arrange
        var featureLabel = Label.Parse("Feature");
        var existingLabels = new List<Label> {featureLabel};

        var line = new ChangeLogLine(TestId, TestVersionId,
            TestProductId, TestText, TestPosition,
            TestCreationDate, existingLabels, Array.Empty<Issue>(), TestUserId);

        // act
        var remainingLabelPlaces = line.AvailableLabelPlaces;

        // assert
        remainingLabelPlaces.Should().Be(4);
    }

    [Fact]
    public void AvailableLabelPlaces_FiveLabelsExists_ReturnsZero()
    {
        // arrange
        var existingLabels = new[]
        {
            Label.Parse("Feature"),
            Label.Parse("Security"),
            Label.Parse("Bug"),
            Label.Parse("Deprecated"),
            Label.Parse("ProxyStrikesBack")
        };

        var line = new ChangeLogLine(TestId, TestVersionId,
            TestProductId, TestText, TestPosition,
            TestCreationDate, existingLabels, Array.Empty<Issue>(), TestUserId);

        // act
        var remainingLabelPlaces = line.AvailableLabelPlaces;

        // assert
        remainingLabelPlaces.Should().Be(0);
    }

    [Fact]
    public void AddLabel_SameLabel_NoDuplicates()
    {
        // arrange
        var featureLabel = Label.Parse("Feature");
        var existingLabels = new List<Label> {featureLabel};

        var line = new ChangeLogLine(TestId, null,
            TestProductId, TestText, TestPosition,
            TestCreationDate, existingLabels, Array.Empty<Issue>(), TestUserId);

        // act
        line.AddLabel(featureLabel);

        // assert
        line.Labels.Count.Should().Be(1);
        line.Labels.First().Should().Be(featureLabel);
    }

    [Fact]
    public void RemoveLabel_ThatExists_LabelRemoved()
    {
        // arrange
        var featureLabel = Label.Parse("Feature");
        var existingLabels = new List<Label> {featureLabel};

        var line = new ChangeLogLine(TestId, null,
            TestProductId, TestText, TestPosition,
            TestCreationDate, existingLabels, Array.Empty<Issue>(), TestUserId);

        // act
        line.RemoveLabel(featureLabel);

        // assert
        line.Labels.Should().BeEmpty();
    }

    [Fact]
    public void RemoveLabel_NoLabelExists_NothingChanged()
    {
        // arrange
        var line = new ChangeLogLine(TestId, null,
            TestProductId, TestText, TestPosition,
            TestUserId, TestCreationDate);

        // act
        line.RemoveLabel(Label.Parse("Feature"));

        // assert
        line.Labels.Should().BeEmpty();
    }
}