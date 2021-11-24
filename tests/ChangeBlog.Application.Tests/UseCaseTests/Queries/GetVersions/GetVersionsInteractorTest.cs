using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetVersions;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetVersions;

public class GetVersionsInteractorTest
{
    private readonly FakeChangeLogDao _changeLogDao;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeUserDao _fakeUserDao;
    private readonly FakeVersionDao _fakeVersionDao;

    public GetVersionsInteractorTest()
    {
        _fakeProductDao = new FakeProductDao();
        _fakeVersionDao = new FakeVersionDao();
        _changeLogDao = new FakeChangeLogDao();
        _fakeUserDao = new FakeUserDao();
    }

    private GetVersionsInteractor CreateInteractor()
    {
        return new GetVersionsInteractor(_fakeProductDao, _fakeVersionDao, _changeLogDao, _fakeUserDao);
    }

    [Fact]
    public async Task GetVersionByVersionId_VersionDoesNotExist_ReturnsNone()
    {
        var notExistingVersionId = Guid.Parse("4c060238-7e2a-4066-ac6a-8a020588e039");
        var interactor = CreateInteractor();

        var version = await interactor.ExecuteAsync(TestAccount.UserId, notExistingVersionId);

        version.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public async Task GetVersionByVersionId_HappyPath_ReturnsVersion()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);
        _fakeVersionDao.Versions.Add(version);
        _fakeProductDao.Products.Add(TestAccount.Product);

        var interactor = CreateInteractor();

        // act
        var requestedVersion = await interactor.ExecuteAsync(TestAccount.UserId, version.Id);

        // assert
        requestedVersion.HasValue.Should().BeTrue();
        requestedVersion.GetValueOrThrow().VersionId.Should().Be(version.Id);
        requestedVersion.GetValueOrThrow().Version.Should().Be(version.Value);
    }

    [Fact]
    public async Task GetVersionByProductIdAndVersion_InvalidVersion_ReturnsNone()
    {
        const string invalidVersion = " ";
        var interactor = CreateInteractor();

        var version = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Product.Id, invalidVersion);

        version.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public async Task GetVersionByProductIdAndVersion_VersionDoesNotExist_ReturnsNone()
    {
        const string invalidVersion = "1.2.3";
        var interactor = CreateInteractor();

        var version = await interactor.ExecuteAsync(TestAccount.UserId, TestAccount.Product.Id, invalidVersion);

        version.HasNoValue.Should().BeTrue();
    }

    [Fact]
    public async Task GetVersionByProductIdAndVersion_HappyPath_ReturnsVersion()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"), OptionalName.Empty,
            TestAccount.UserId);
        _fakeVersionDao.Versions.Add(version);
        _fakeProductDao.Products.Add(TestAccount.Product);

        var interactor = CreateInteractor();

        // act
        var requestedVersion = await interactor.ExecuteAsync(TestAccount.UserId, version.ProductId, version.Value);

        // assert
        requestedVersion.HasValue.Should().BeTrue();
        requestedVersion.GetValueOrThrow().VersionId.Should().Be(version.Id);
        requestedVersion.GetValueOrThrow().Version.Should().Be(version.Value);
    }

    [Fact]
    public async Task GetVersionByProductIdAndVersion_HappyPath_ResponseModelProperlyPopulated()
    {
        // arrange
        _fakeUserDao.Users.Add(TestAccount.User);
        var versionId = Guid.Parse("d1959237-0253-42c8-a817-77fd7fa4f126");
        var version = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty,
            DateTime.Parse("2021-09-10T18:00:00"), TestAccount.UserId, DateTime.Parse("2021-09-10T14:00:00"), null);
        _fakeVersionDao.Versions.Add(version);
        _fakeProductDao.Products.Add(TestAccount.Product);

        var interactor = CreateInteractor();

        // act
        var requestedVersion = await interactor.ExecuteAsync(TestAccount.UserId, version.ProductId, version.Value);

        // assert
        requestedVersion.HasValue.Should().BeTrue();
        requestedVersion.GetValueOrThrow().VersionId.Should().Be(version.Id);
        requestedVersion.GetValueOrThrow().Version.Should().Be(version.Value);
        requestedVersion.GetValueOrThrow().DeletedAt.HasValue.Should().BeFalse();
        requestedVersion.GetValueOrThrow().CreatedAt.Should().Be(DateTimeOffset.Parse("2021-09-10T16:00:00+02:00"));
        requestedVersion.GetValueOrThrow().ReleasedAt.HasValue.Should().BeTrue();
        requestedVersion.GetValueOrThrow().ReleasedAt.Should().Be(DateTimeOffset.Parse("2021-09-10T20:00:00+02:00"));
        requestedVersion.GetValueOrThrow().ProductId.Should().Be(TestAccount.Product.Id);
        requestedVersion.GetValueOrThrow().AccountId.Should().Be(TestAccount.Id);
        requestedVersion.GetValueOrThrow().ChangeLogs.Should().BeEmpty();
    }
}