using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.AddProduct;
using ChangeBlog.Domain;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AddProduct;

public class AddProductInteractorTests
{
    private readonly FakeAccountDao _fakeAccountDao;
    private readonly FakeProductDao _fakeProductDao;
    private readonly FakeVersioningSchemeDao _fakeVersioningSchemeDao;
    private readonly Mock<IAddProductOutputPort> _outputPortMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;

    public AddProductInteractorTests()
    {
        _fakeAccountDao = new FakeAccountDao();
        _fakeVersioningSchemeDao = new FakeVersioningSchemeDao();
        _fakeProductDao = new FakeProductDao();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _outputPortMock = new Mock<IAddProductOutputPort>(MockBehavior.Strict);
    }

    private AddProductInteractor CreateInteractor()
    {
        return new(_fakeAccountDao,
            _fakeVersioningSchemeDao,
            _fakeProductDao,
            _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task CreateProduct_Successful()
    {
        // arrange
        var account = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
        _fakeAccountDao.Accounts.Add(account);
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
        var productRequestModel = new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value,
            TestAccount.CustomVersioningScheme.Id, "en", TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>(), It.IsAny<Guid>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(
            m => m.Created(
                It.Is<Guid>(x => x == TestAccount.Id),
                It.IsAny<Guid>()), Times.Once);

        _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_NotExistingAccount_AccountDoesNotExistsOutput()
    {
        // arrange
        var productRequestModel =
            new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, "en", TestAccount.UserId);
        var createProductInteractor = CreateInteractor();
        _outputPortMock.Setup(m => m.AccountDoesNotExist(It.IsAny<Guid>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(m => m.AccountDoesNotExist(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_DeletedAccount_AccountDeletedOutput()
    {
        // arrange
        var deletedAccount = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            DateTime.Parse("2021-04-04"));
        _fakeAccountDao.Accounts.Add(deletedAccount);

        var productRequestModel =
            new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, "en", TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.AccountDeleted(It.IsAny<Guid>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(m
            => m.AccountDeleted(It.Is<Guid>(x => x == TestAccount.Id)), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_InvalidName_InvalidNameOutput()
    {
        // arrange
        _fakeAccountDao.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            null));

        var productRequestModel = new ProductRequestModel(TestAccount.Id, "", null, "en", TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.InvalidName(It.IsAny<string>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(m => m.InvalidName(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ProductExists_ProductAlreadyExistsOutput()
    {
        // arrange
        _fakeAccountDao.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            null));
        _fakeProductDao.Products.Add(new Product(TestAccount.Id, TestAccount.Name,
            TestAccount.Product.VersioningScheme, TestAccount.UserId, TestAccount.Product.LanguageCode,
            DateTime.Parse("2021-04-04")));

        var productRequestModel =
            new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, "en", TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.ProductAlreadyExists(It.IsAny<Guid>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(m => m.ProductAlreadyExists(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_NotExistingVersioningScheme_VersioningSchemeDoesNotExistOutput()
    {
        // arrange
        _fakeAccountDao.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            null));
        var notExistingVersioningSchemeId = Guid.Parse("3984bcf2-9930-4d41-984e-b72ccc6d6c87");

        var productRequestModel =
            new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, notExistingVersioningSchemeId, "en",
                TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.VersioningSchemeDoesNotExist(It.IsAny<Guid>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(
            m => m.VersioningSchemeDoesNotExist(It.Is<Guid>(x => x == notExistingVersioningSchemeId)), Times.Once);
    }

    [Fact]
    public async Task CreateProduct_EmptyLangCode_NotSupportedLanguageCodeOutput()
    {
        // arrange
        _fakeAccountDao.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            null));
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.DefaultScheme);

        var productRequestModel =
            new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, "",
                TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.NotSupportedLanguageCode(It.IsAny<string>(), It.IsAny<IList<string>>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(
            m => m.NotSupportedLanguageCode(It.Is<string>(x => x == string.Empty), It.IsAny<IList<string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateProduct_NotSupportedLangCode_NotSupportedLanguageCodeOutput()
    {
        // arrange
        _fakeAccountDao.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            null));
        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.DefaultScheme);

        var productRequestModel =
            new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, "Deitsch",
                TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.NotSupportedLanguageCode(It.IsAny<string>(), It.IsAny<IList<string>>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(
            m => m.NotSupportedLanguageCode(It.Is<string>(x => x == "deitsch"), It.IsAny<IList<string>>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateProduct_ConflictWhenSaving_ConflictOutput()
    {
        // arrange
        _fakeAccountDao.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
            null));

        _fakeVersioningSchemeDao.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
        _fakeProductDao.Conflict = new ConflictStub();

        var productRequestModel = new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value,
            TestAccount.CustomVersioningScheme.Id, "en", TestAccount.UserId);
        var createProductInteractor = CreateInteractor();

        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        // act
        await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }
}