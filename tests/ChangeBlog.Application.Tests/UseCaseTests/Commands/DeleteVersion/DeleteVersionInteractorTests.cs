using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.DeleteVersion;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.DeleteVersion;

public class DeleteVersionInteractorTests
{
    private readonly Mock<IDeleteVersionOutputPort> _outputPortMock;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeVersionDao _fakeVersionDao;

    public DeleteVersionInteractorTests()
    {
        _fakeVersionDao = new FakeVersionDao();
        _fakeProductDao = new FakeProductDao();
        _outputPortMock = new Mock<IDeleteVersionOutputPort>(MockBehavior.Strict);
    }

    private DeleteVersionInteractor CreateInteractor() => new(_fakeVersionDao, _fakeProductDao);

    [Fact]
    public async Task DeleteVersion_VersionDoesNotExist_VersionDoesNotExistOutput()
    {
        // arrange
        var notExistingVersionId = Guid.Parse("11cbae99-5cc0-4268-8d3f-31961822133c");
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.VersionDoesNotExist(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, notExistingVersionId);

        // assert
        _outputPortMock.Verify(m => m.VersionDoesNotExist(It.Is<Guid>(x => x == notExistingVersionId)), Times.Once);
    }

    [Fact]
    public async Task DeleteVersion_DeletedVersion_VersionAlreadyDeletedOutput()
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
    public async Task DeleteVersion_ReleasedVersion_VersionAlreadyReleasedOutput()
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
    public async Task DeleteVersion_ProductClosedExist_ProductClosedOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId);
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.RelatedProductClosed(It.IsAny<Guid>()));

        _fakeProductDao.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, Name.Parse("test product"),
            TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
            DateTime.Parse("2021-05-13"),
            DateTime.Parse("2021-05-13")));
        _fakeVersionDao.Versions.Add(version);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.RelatedProductClosed(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task DeleteVersion_ConflictWhenDeleting_ConflictOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId);
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        _fakeProductDao.Products.Add(TestAccount.Product);
        _fakeVersionDao.Versions.Add(version);
        _fakeVersionDao.Conflict = new ConflictStub();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }

    [Fact]
    public async Task DeleteVersion_HappyPath_VersionDeletedOutput()
    {
        // arrange
        var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), OptionalName.Empty,
            TestAccount.UserId);
        var interactor = CreateInteractor();
        _outputPortMock.Setup(m => m.VersionDeleted(It.IsAny<Guid>()));

        _fakeProductDao.Products.Add(TestAccount.Product);
        _fakeVersionDao.Versions.Add(version);

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

        // assert
        _outputPortMock.Verify(m => m.VersionDeleted(It.Is<Guid>(x => x == version.Id)), Times.Once);
    }
}