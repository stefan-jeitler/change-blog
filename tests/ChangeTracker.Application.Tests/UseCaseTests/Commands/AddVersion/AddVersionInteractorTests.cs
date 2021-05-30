using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.AddVersion;
using ChangeTracker.Application.UseCases.Commands.SharedModels;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddVersion
{
    public class AddVersionInteractorTests
    {
        private readonly Mock<IAddVersionOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddVersionInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IAddVersionOutputPort>(MockBehavior.Strict);
        }

        private AddVersionInteractor CreateInteractor() =>
            new(_versionDaoStub, _productDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public async Task CreateVersion_HappyPath_Successful()
        {
            // arrange
            _productDaoStub.Products.Add(TestAccount.Product);
            var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "");
            var addVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));

            // act
            await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.Is<Guid>(x => x != Guid.Empty)));
        }

        [Fact]
        public async Task CreateVersion_VersionWithWhitespaceInTheMiddle_InvalidVersionFormatOutput()
        {
            // arrange
            _productDaoStub.Products.Add(TestAccount.Product);
            var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1. .3", "");
            var addVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

            // act
            await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .3")));
        }

        [Fact]
        public async Task CreateVersion_NoProductExists_ProductDoesNotExistOutput()
        {
            // arrange
            var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "");
            var addVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ProductDoesNotExist(It.IsAny<Guid>()));

            // act
            await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProductDoesNotExist(It.Is<Guid>(x => x == TestAccount.Product.Id)));
        }

        [Fact]
        public async Task CreateVersion_ProductIsClosed_ProductClosedOutput()
        {
            // arrange
            var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, DateTime.Parse("2021-04-04"),
                DateTime.Parse("2021-05-13"));
            _productDaoStub.Products.Add(product);

            var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "12.1", "");
            var addVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ProductClosed(It.IsAny<Guid>()));

            // act
            await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProductClosed(It.Is<Guid>(x => x == product.Id)));
        }

        [Fact]
        public async Task CreateVersion_VersionSchemeMismatch_VersionDoesNotMatchSchemeOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, DateTime.Parse("2021-04-04"), null));

            var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "12*", "");
            var addVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotMatchScheme(It.IsAny<string>(), It.IsAny<string>()));

            // act
            await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotMatchScheme(It.Is<string>(x => x == "12*"), 
                It.Is<string>(x => x == TestAccount.CustomVersioningScheme.Name.Value)));
        }

        [Fact]
        public async Task CreateVersion_VersionAlreadyExists_VersionAlreadyExistsOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, DateTime.Parse("2021-04-04"), null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = ClVersionValue.Parse("1.2");
            _versionDaoStub.Versions.Add(new ClVersion(versionId, TestAccount.Product.Id,
                version, OptionalName.Empty, DateTime.Parse("2021-04-12"), TestAccount.UserId,
                DateTime.Parse("2021-04-12"), DateTime.Parse("2021-04-12")));

            var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, version, "");
            var addVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionAlreadyExists(It.IsAny<Guid>()));

            // act
            await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyExists(It.Is<Guid>(x => x == versionId)));
        }

        [Fact]
        public async Task CreateVersion_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            _productDaoStub.Products.Add(TestAccount.Product);
            var versionRequestModel = new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "");
            var addVersionInteractor = CreateInteractor();
            _versionDaoStub.ProduceConflict = true;
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await addVersionInteractor.ExecuteAsync(_outputPortMock.Object, versionRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()));
        }
    }
}