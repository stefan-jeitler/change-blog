using System;
using ChangeBlog.Application.UseCases.Versions.AssignAllPendingLinesToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AssignAllPendingLinesToVersion;

public class VersionAssignmentRequestModelTests
{
    private Guid _testProductId;
    private string _testVersion;

    public VersionAssignmentRequestModelTests()
    {
        _testProductId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
        _testVersion = "1.2.3";
    }

    private VersionAssignmentRequestModel CreateRequestModel()
    {
        return new VersionAssignmentRequestModel(_testProductId, _testVersion);
    }

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.ProductId.Should().Be(_testProductId);
        requestModel.Version.Should().Be(_testVersion);
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
}