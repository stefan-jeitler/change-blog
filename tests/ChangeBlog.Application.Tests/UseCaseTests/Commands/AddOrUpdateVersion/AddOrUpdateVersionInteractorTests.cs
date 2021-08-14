using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Common;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AddOrUpdateVersion
{
    public class AddOrUpdateVersionInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddOrUpdateVersionOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddOrUpdateVersionInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _outputPortMock = new Mock<IAddOrUpdateVersionOutputPort>(MockBehavior.Strict);
        }

        private AddOrUpdateVersionInteractor CreateInteractor() =>
            new(_productDaoStub, _versionDaoStub,
                _unitOfWorkMock.Object, _changeLogDaoStub, _changeLogDaoStub);

        [Fact]
        public async Task UpdateVersion_HappyPath_SuccessfullyUpdated()
        {
            // arrange
            var requestModel =
                new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "catchy name",
                    new List<ChangeLogLineRequestModel>(0));

            var existingVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
                OptionalName.Empty, TestAccount.UserId);

            _versionDaoStub.Versions.Add(existingVersion);
            _productDaoStub.Products.Add(TestAccount.Product);
            var interactor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionUpdated(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionUpdated(It.Is<Guid>(x => x == existingVersion.Id)), Times.Once);
            _versionDaoStub.Versions.Should().HaveCount(1);
            _versionDaoStub.Versions.Should().Contain(x => x.Id == existingVersion.Id && x.Name == "catchy name");
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

            _versionDaoStub.Versions.Add(existingVersion);
            _productDaoStub.Products.Add(TestAccount.Product);
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

            _versionDaoStub.Versions.Add(existingVersion);
            _productDaoStub.Products.Add(TestAccount.Product);
            var interactor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionAlreadyReleased(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionAlreadyReleased(It.Is<Guid>(x => x == existingVersion.Id)),
                Times.Once);
        }

        [Fact]
        public async Task UpdateVersion_ProductClosed_RelatedProductClosedOutput()
        {
            // arrange
            var product = new Product(TestAccount.Account.Id, TestAccount.Product.Name,
                TestAccount.DefaultScheme, TestAccount.UserId, TestAccount.Product.LanguageCode,
                TestAccount.Product.CreatedAt).Close();

            var requestModel =
                new VersionRequestModel(TestAccount.UserId, product.Id, "1.2.3", "catchy name",
                    new List<ChangeLogLineRequestModel>(0));

            var existingVersion = new ClVersion(product.Id, ClVersionValue.Parse("1.2.3"),
                OptionalName.Empty, TestAccount.UserId);

            _versionDaoStub.Versions.Add(existingVersion);


            _productDaoStub.Products.Add(product);
            var interactor = CreateInteractor();

            _outputPortMock.Setup(m => m.RelatedProductClosed(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.RelatedProductClosed(It.Is<Guid>(x => x == product.Id)),
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

            _versionDaoStub.Versions.Add(existingVersion);


            _productDaoStub.Products.Add(product);
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

            _versionDaoStub.Versions.Add(existingVersion);


            _productDaoStub.Products.Add(product);
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
}
