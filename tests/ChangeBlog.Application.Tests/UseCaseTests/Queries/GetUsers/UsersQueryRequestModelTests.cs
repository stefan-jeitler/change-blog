using System;
using ChangeBlog.Application.UseCases.Queries.GetUsers;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetUsers;

public class UsersQueryRequestModelTests
{
    private readonly Guid? _testLastUserId;
    private Guid _testAccountId;
    private ushort _testCount;
    private Guid _testUserId;

    public UsersQueryRequestModelTests()
    {
        _testUserId = TestAccount.UserId;
        _testAccountId = TestAccount.Id;
        _testLastUserId = Guid.Parse("33f9a7a4-5a0b-4ac8-b074-d97e13e8596c");
        _testCount = 100;
    }

    private UsersQueryRequestModel CreateRequestModel()
    {
        return new UsersQueryRequestModel(_testUserId, _testAccountId, _testLastUserId, _testCount);
    }

    [Fact]
    public void Create_HappyPath_Successful()
    {
        var requestModel = CreateRequestModel();

        requestModel.UserId.Should().Be(_testUserId);
        requestModel.AccountId.Should().Be(_testAccountId);
        requestModel.LastUserId.Should().Be(_testLastUserId);
        requestModel.Limit.Should().Be(_testCount);
    }

    [Fact]
    public void Create_WithEmptyUserId_ArgumentException()
    {
        _testUserId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyAccountId_ArgumentException()
    {
        _testAccountId = Guid.Empty;

        var act = CreateRequestModel;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithTooHighCountValue_ValueCapped()
    {
        _testCount = 500;

        var requestModel = CreateRequestModel();

        requestModel.Limit.Should().Be(UsersQueryRequestModel.MaxLimit);
    }
}