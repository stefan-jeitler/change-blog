using System;
using ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AssignPendingLineToVersion;

public class VersionAssignmentRequestModelTests
{
    private Guid _testLineId;
    private Guid _testProductId;
    private string _testVersion;

    public VersionAssignmentRequestModelTests()
    {
        _testProductId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
        _testVersion = "1.2.3";
        _testLineId = Guid.Parse("1763b2e7-9835-4992-8f73-8c2026530b2c");
    }

    private VersionAssignmentRequestModel CreateRequestModel() => new(_testProductId, _testVersion, _testLineId);

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.ProductId.Should().Be(_testProductId);
        requestModel.Version.Should().Be(_testVersion);
        requestModel.ChangeLogLineId.Should().Be(_testLineId);
    }

    [Fact]
    public void Create_WithEmptyProductId_ArgumentException()
    {
        _testProductId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullVersion_ArgumentNullException()
    {
        _testVersion = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithEmptyLineId_ArgumentException()
    {
        _testLineId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }
}