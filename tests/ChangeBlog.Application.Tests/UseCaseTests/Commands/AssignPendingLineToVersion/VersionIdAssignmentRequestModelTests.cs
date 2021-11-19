using System;
using ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AssignPendingLineToVersion;

public class VersionIdAssignmentRequestModelTests
{
    private Guid _testLineId;
    private Guid _testVersionId;

    public VersionIdAssignmentRequestModelTests()
    {
        _testVersionId = Guid.Parse("30027f7d-91e4-4d08-afdc-a21d19656bb6");
        _testLineId = Guid.Parse("1763b2e7-9835-4992-8f73-8c2026530b2c");
    }

    private VersionIdAssignmentRequestModel CreateRequestModel() => new(_testVersionId, _testLineId);

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.VersionId.Should().Be(_testVersionId);
        requestModel.ChangeLogLineId.Should().Be(_testLineId);
    }

    [Fact]
    public void Create_WithEmptyVersionId_ArgumentException()
    {
        _testVersionId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyLineId_ArgumentException()
    {
        _testLineId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }
}