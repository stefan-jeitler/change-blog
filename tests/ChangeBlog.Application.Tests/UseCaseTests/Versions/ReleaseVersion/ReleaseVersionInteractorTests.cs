using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Versions.ReleaseVersion;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Versions.ReleaseVersion;

public class ReleaseVersionInteractorTests
{
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeVersionDao _fakeVersionDao;
    private readonly Mock<IReleaseVersionOutputPort> _outputPortMock;

    public ReleaseVersionInteractorTests()
    {
        _fakeVersionDao = new FakeVersionDao();
        _fakeProductDao = new FakeProductDao();
        _outputPortMock = new Mock<IReleaseVersionOutputPort>(MockBehavior.Strict);
    }

    private ReleaseVersionInteractor CreateInteractor() => new(_fakeVersionDao, _fakeProductDao);

    [Fact]
    public async Task ReleaseVersion_NotExistingVersion_VersionDoesNotExistOutput()
    {
        // arrange
        var notExistingVersion = Guid.Parse("7a9fef4d-cf90-4eaf-9c7e-ee639b88939b");
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.VersionDoesNotExist(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, notExistingVersion);

        // assert
        _outputPortMock.Verify(m => m.VersionDoesNotExist(It.Is<Guid>(x => x == notExistingVersion)), Times.Once);
    }

    [Fact]
    public async Task ReleaseVersion_DeletedVersion_VersionDeletedOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId, null,
            DateTime.Parse("2021-05-13"));
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.VersionAlreadyDeleted(It.IsAny<Guid>()));
        _fakeVersionDao.Versions.Add(version);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionAlreadyDeleted(It.Is<Guid>(x => x == version.Id)), Times.Once);
    }

    [Fact]
    public async Task ReleaseVersion_ReleasedVersion_VersionAlreadyReleasedOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId,
            DateTime.Parse("2021-05-13"));
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<Guid>()));
        _fakeVersionDao.Versions.Add(version);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionAlreadyReleased(It.Is<Guid>(x => x == version.Id)), Times.Once);
    }

    [Fact]
    public async Task ReleaseVersion_RelatedProductIsFreezed_RelatedProductFreezedOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId);
        var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            TestAccount.Product.CreatedAt,
            DateTime.Parse("2021-05-13"));
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.RelatedProductFreezed(It.IsAny<Guid>()));
        _fakeVersionDao.Versions.Add(version);
        _fakeProductDao.Products.Add(product);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.RelatedProductFreezed(It.Is<Guid>(x => x == product.Id)), Times.Once);
    }

    [Fact]
    public async Task ReleaseVersion_ConflictWhenRelease_ConflictOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId);
        var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            TestAccount.Product.CreatedAt, null);
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));
        _fakeVersionDao.Versions.Add(version);
        _fakeVersionDao.Conflict = new ConflictStub();
        _fakeProductDao.Products.Add(product);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }

    [Fact]
    public async Task ReleaseVersion_HappyPath_ReleasedOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId);
        var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            TestAccount.Product.CreatedAt, null);
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.VersionReleased(It.IsAny<Guid>()));
        _fakeVersionDao.Versions.Add(version);
        _fakeProductDao.Products.Add(product);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionReleased(It.Is<Guid>(x => x == version.Id)), Times.Once);
    }
}