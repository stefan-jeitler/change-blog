using System;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.ReleaseVersion;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Version;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.ReleaseVersion
{
    public class ReleaseVersionInteractorTests
    {
        private readonly Mock<IReleaseVersionOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly VersionDaoStub _versionDaoStub;

        public ReleaseVersionInteractorTests()
        {
            _versionDaoStub = new VersionDaoStub();
            _productDaoStub = new ProductDaoStub();
            _outputPortMock = new Mock<IReleaseVersionOutputPort>(MockBehavior.Strict);
        }

        private ReleaseVersionInteractor CreateInteractor()
        {
            return new(_versionDaoStub, _productDaoStub);
        }

        [Fact]
        public async Task ReleaseVersion_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            var notExistingVersion = Guid.Parse("7a9fef4d-cf90-4eaf-9c7e-ee639b88939b");
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, notExistingVersion);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_DeletedVersion_VersionDeletedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"), null,
                DateTime.Parse("2021-05-13"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionDeleted());
            _versionDaoStub.Versions.Add(version);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionDeleted(), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_ReleasedVersion_VersionAlreadyReleasedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"),
                DateTime.Parse("2021-05-13"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionAlreadyReleased());
            _versionDaoStub.Versions.Add(version);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyReleased(), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_RelatedProductClosed_RelatedProductClosedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"));
            var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Product.CreatedAt,
                DateTime.Parse("2021-05-13"));
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.RelatedProductClosed(It.IsAny<Guid>()));
            _versionDaoStub.Versions.Add(version);
            _productDaoStub.Products.Add(product);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.RelatedProductClosed(It.Is<Guid>(x => x == product.Id)), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_ConflictWhenRelease_ConflictOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"));
            var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Product.CreatedAt, null);
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _versionDaoStub.Versions.Add(version);
            _versionDaoStub.ProduceConflict = true;
            _productDaoStub.Products.Add(product);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ReleaseVersion_HappyPath_ReleasedOutput()
        {
            // arrange
            var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.23"));
            var product = new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.UserId, TestAccount.Product.CreatedAt, null);
            var interactor = CreateInteractor();
            _outputPortMock.Setup(m => m.VersionReleased(It.IsAny<Guid>()));
            _versionDaoStub.Versions.Add(version);
            _productDaoStub.Products.Add(product);

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, version.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionReleased(It.Is<Guid>(x => x == version.Id)), Times.Once);
        }
    }
}