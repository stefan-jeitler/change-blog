using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.OutputPorts;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Versions.AddOrUpdateVersion;

public class AddOrUpdateVersionInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeVersionDao _fakeVersionDao;
    private readonly Mock<IAddOrUpdateVersionOutputPort> _outputPortMock;
    private readonly Mock<IBusinessTransaction> _unitOfWorkMock;

    public AddOrUpdateVersionInteractorTests()
    {
        _fakeProductDao = new FakeProductDao();
        _fakeVersionDao = new FakeVersionDao();
        _fakeChangeLogDao = new FakeChangeLogDao();
        _unitOfWorkMock = new Mock<IBusinessTransaction>();
        _outputPortMock = new Mock<IAddOrUpdateVersionOutputPort>(MockBehavior.Strict);
    }

    private AddOrUpdateVersionInteractor CreateInteractor() =>
        new(_fakeProductDao, _fakeVersionDao,
            _unitOfWorkMock.Object, _fakeChangeLogDao, _fakeChangeLogDao);

    [Fact]
    public async Task UpdateVersion_HappyPath_SuccessfullyUpdated()
    {
        // arrange
        var requestModel =
            new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "catchy name",
                new List<ChangeLogLineRequestModel>(0));

        var existingVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, TestAccount.UserId);

        _fakeVersionDao.Versions.Add(existingVersion);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();

        _outputPortMock.Setup(m => m.VersionUpdated(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.VersionUpdated(It.Is<Guid>(x => x == existingVersion.Id)), Times.Once);
        _fakeVersionDao.Versions.Should().HaveCount(1);
        _fakeVersionDao.Versions.Should().Contain(x => x.Id == existingVersion.Id && x.Name == "catchy name");
    }

    [Fact]
    public async Task UpdateVersion_VersionDeleted_VersionDeletedOutput()
    {
        // arrange
        var requestModel =
            new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "catchy name",
                new List<ChangeLogLineRequestModel>(0));

        var existingVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, TestAccount.UserId, null, DateTime.Parse("2021-06-03"));

        _fakeVersionDao.Versions.Add(existingVersion);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();

        _outputPortMock.Setup(m => m.VersionAlreadyDeleted(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.VersionAlreadyDeleted(It.Is<Guid>(x => x == existingVersion.Id)), Times.Once);
    }

    [Fact]
    public async Task UpdateVersion_VersionAlreadyReleased_VersionReleasedOutput()
    {
        // arrange
        var requestModel =
            new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "catchy name",
                new List<ChangeLogLineRequestModel>(0));

        var existingVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, TestAccount.UserId, DateTime.Parse("2021-06-03"));

        _fakeVersionDao.Versions.Add(existingVersion);
        _fakeProductDao.Products.Add(TestAccount.Product);
        var interactor = CreateInteractor();

        _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.VersionAlreadyReleased(It.Is<Guid>(x => x == existingVersion.Id)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVersion_ProductFreezed_RelatedProductFreezedOutput()
    {
        // arrange
        var product = new Product(TestAccount.Account.Id, TestAccount.Product.Name,
            TestAccount.DefaultScheme, TestAccount.UserId, TestAccount.Product.LanguageCode,
            TestAccount.Product.CreatedAt).Freeze();

        var requestModel =
            new VersionRequestModel(TestAccount.UserId, product.Id, "1.2.3", "catchy name",
                new List<ChangeLogLineRequestModel>(0));

        var existingVersion = new ClVersion(product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, TestAccount.UserId);

        _fakeVersionDao.Versions.Add(existingVersion);


        _fakeProductDao.Products.Add(product);
        var interactor = CreateInteractor();

        _outputPortMock.Setup(m => m.RelatedProductFreezed(It.IsAny<Guid>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.RelatedProductFreezed(It.Is<Guid>(x => x == product.Id)),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVersion_VersioningSchemeMismatch_VersionDoesNotMatchVersioningSchemeOutput()
    {
        // arrange
        var product = new Product(TestAccount.Account.Id, TestAccount.Product.Name,
            TestAccount.DefaultScheme, TestAccount.UserId, TestAccount.Product.LanguageCode,
            TestAccount.Product.CreatedAt);

        var requestModel =
            new VersionRequestModel(TestAccount.UserId, product.Id, "1", "catchy name",
                new List<ChangeLogLineRequestModel>(0));

        var existingVersion = new ClVersion(product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, TestAccount.UserId);

        _fakeVersionDao.Versions.Add(existingVersion);


        _fakeProductDao.Products.Add(product);
        var interactor = CreateInteractor();

        _outputPortMock.Setup(m => m.VersionDoesNotMatchScheme(It.IsAny<string>(), It.IsAny<string>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.VersionDoesNotMatchScheme(It.Is<string>(x => x == "1"), It.IsAny<string>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVersion_VersionNameWithOneChar_InvalidVersionNameOutput()
    {
        // arrange
        var product = new Product(TestAccount.Account.Id, TestAccount.Product.Name,
            TestAccount.DefaultScheme, TestAccount.UserId, TestAccount.Product.LanguageCode,
            TestAccount.Product.CreatedAt);

        var requestModel =
            new VersionRequestModel(TestAccount.UserId, product.Id, "1.2.3", "v",
                new List<ChangeLogLineRequestModel>(0));

        var existingVersion = new ClVersion(product.Id, ClVersionValue.Parse("1.2.3"),
            OptionalName.Empty, TestAccount.UserId);

        _fakeVersionDao.Versions.Add(existingVersion);


        _fakeProductDao.Products.Add(product);
        var interactor = CreateInteractor();

        _outputPortMock.Setup(m => m.InvalidVersionName(It.IsAny<string>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.InvalidVersionName(It.Is<string>(x => x == "v")),
            Times.Once);
    }

    [Fact]
    public async Task UpdateVersion_EmptyVersionValue_InvalidVersionFormatOutput()
    {
        // arrange
        var requestModel =
            new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "", string.Empty,
                new List<ChangeLogLineRequestModel>(0));

        var interactor = CreateInteractor();

        _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

        // assert
        _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "")),
            Times.Once);
    }
}