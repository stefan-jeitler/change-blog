using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.UpdateVersion;

public class VersionRequestModelTests
{
    private string _testName;
    private Guid _testProductId;
    private Guid _testUserId;
    private string _testVersion;

    public VersionRequestModelTests()
    {
        _testProductId = Guid.Parse("53161c63-e6c9-4908-8dac-9940896817c9");
        _testUserId = Guid.Parse("220cac7a-a4cd-41ed-9f3c-5118a97f75a2");
        _testName = "Release";
        _testVersion = "1.2.3";
    }

    private VersionRequestModel CreateRequestModel()
    {
        return new VersionRequestModel(_testUserId, _testProductId, _testVersion, _testName,
            new List<ChangeLogLineRequestModel>());
    }

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var updateVersionRequestModel = CreateRequestModel();

        updateVersionRequestModel.ProductId.Should().Be(_testProductId);
        updateVersionRequestModel.UserId.Should().Be(_testUserId);
        updateVersionRequestModel.Version.Should().Be(_testVersion);
        updateVersionRequestModel.Name.Should().Be(_testName);
    }

    [Fact]
    public void Create_WithEmptyProductId_ArgumentException()
    {
        _testProductId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyUserId_ArgumentException()
    {
        _testUserId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullName_NoException()
    {
        _testName = null;

        var requestModel = CreateRequestModel();

        requestModel.Name.Should().BeNull();
    }

    [Fact]
    public void Create_WithNullVersion_ArgumentException()
    {
        _testVersion = null;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentNullException>();
    }
}