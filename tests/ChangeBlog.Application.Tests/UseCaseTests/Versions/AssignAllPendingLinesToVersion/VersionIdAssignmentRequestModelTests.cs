using System;
using ChangeBlog.Application.UseCases.Versions.AssignAllPendingLinesToVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Versions.AssignAllPendingLinesToVersion;

public class VersionIdAssignmentRequestModelTests
{
    private Guid _testProductId;
    private Guid _testVersionId;

    public VersionIdAssignmentRequestModelTests()
    {
        _testProductId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
        _testVersionId = Guid.Parse("30027f7d-91e4-4d08-afdc-a21d19656bb6");
    }

    private VersionIdAssignmentRequestModel CreateRequestModel()
    {
        return new VersionIdAssignmentRequestModel(_testProductId, _testVersionId);
    }

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.ProductId.Should().Be(_testProductId);
        requestModel.VersionId.Should().Be(_testVersionId);
    }

    [Fact]
    public void Create_WithEmptyProductId_ArgumentException()
    {
        _testProductId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyVersionId_ArgumentException()
    {
        _testVersionId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }
}