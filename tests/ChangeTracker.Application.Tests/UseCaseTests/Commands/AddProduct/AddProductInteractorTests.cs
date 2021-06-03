using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.Domain;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddProduct
{
    public class AddProductInteractorTests
    {
        private readonly AccountDaoStub _accountDaoStub;
        private readonly Mock<IAddProductOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersioningSchemeDaoStub _versioningSchemeDaoStub;

        public AddProductInteractorTests()
        {
            _accountDaoStub = new AccountDaoStub();
            _versioningSchemeDaoStub = new VersioningSchemeDaoStub();
            _productDaoStub = new ProductDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IAddProductOutputPort>(MockBehavior.Strict);
        }

        private AddProductInteractor CreateInteractor() =>
            new(_accountDaoStub,
                _versioningSchemeDaoStub,
                _productDaoStub,
                _unitOfWorkMock.Object);

        [Fact]
        public async Task CreateProduct_Successful()
        {
            // arrange
            var account = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            _accountDaoStub.Accounts.Add(account);
            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
            var productRequestModel = new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id, TestAccount.UserId);
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
                new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, TestAccount.UserId);
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
            _accountDaoStub.Accounts.Add(deletedAccount);

            var productRequestModel =
                new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, TestAccount.UserId);
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
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));

            var productRequestModel = new ProductRequestModel(TestAccount.Id, "", null, TestAccount.UserId);
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
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));
            _productDaoStub.Products.Add(new Product(TestAccount.Id, TestAccount.Name,
                TestAccount.Product.VersioningScheme, TestAccount.UserId,
                DateTime.Parse("2021-04-04")));

            var productRequestModel =
                new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, null, TestAccount.UserId);
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
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));
            var notExistingVersioningSchemeId = Guid.Parse("3984bcf2-9930-4d41-984e-b72ccc6d6c87");

            var productRequestModel =
                new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value, notExistingVersioningSchemeId,
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
        public async Task CreateProduct_ConflictWhenSaving_ConflictOutput()
        {
            // arrange
            _accountDaoStub.Accounts.Add(new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate,
                null));

            _versioningSchemeDaoStub.VersioningSchemes.Add(TestAccount.CustomVersioningScheme);
            _productDaoStub.ProduceConflict = true;

            var productRequestModel = new ProductRequestModel(TestAccount.Id, TestAccount.Name.Value,
                TestAccount.CustomVersioningScheme.Id, TestAccount.UserId);
            var createProductInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await createProductInteractor.ExecuteAsync(_outputPortMock.Object, productRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }
    }
}