using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetLatestVersion;
using ChangeBlog.Application.UseCases.Queries.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Common;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetLatestVersion
{
    public class GetLatestVersionInteractorTests
    {
        private readonly Mock<IGetLatestVersionOutputPort> _outputPortMock;
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly VersionDaoStub _versionDaoStub;
        private readonly ProductDaoStub _productDaoStub;
        private readonly UserDaoStub _userDaoStub;

        public GetLatestVersionInteractorTests()
        {
            _outputPortMock = new Mock<IGetLatestVersionOutputPort>(MockBehavior.Strict);
            _changeLogDaoStub = new ChangeLogDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _productDaoStub = new ProductDaoStub();
            _userDaoStub = new UserDaoStub();
        }

        private GetLatestVersionInteractor CreateInteractor() =>
            new(_versionDaoStub, _changeLogDaoStub, _productDaoStub, _userDaoStub);

        [Fact]
        public async Task GetLatestVersion_HappyPath_Successful()
        {
            // arrange
            var versionId = Guid.Parse("d8d1ab5d-670d-4370-84d5-2a260755e45c");
            
            var latestVersion = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("2.2.2"), OptionalName.Empty,
                null, TestAccount.UserId, DateTime.Parse("2021-08-02"), null);

            var oldVersion = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.1.1"), OptionalName.Empty,
                null, TestAccount.UserId, DateTime.Parse("2021-08-01"), null);

            _versionDaoStub.Versions.Add(latestVersion);
            _versionDaoStub.Versions.Add(oldVersion);

            var lines = new[]
            {
                new ChangeLogLine(versionId, TestAccount.Product.Id, ChangeLogText.Parse("Test text"), 0, TestAccount.UserId),
                new ChangeLogLine(versionId, TestAccount.Product.Id, ChangeLogText.Parse("Test text 2"), 1, TestAccount.UserId)
            };

            _changeLogDaoStub.ChangeLogs.AddRange(lines);
            _productDaoStub.Products.Add(TestAccount.Product);
            _userDaoStub.Users.Add(TestAccount.User);

            var interactor = CreateInteractor();
            _outputPortMock.Setup(x => x.VersionFound(It.IsAny<VersionResponseModel>()));
            
            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, TestAccount.Product.Id);

            // assert
            _outputPortMock.Verify(m => m.VersionFound(It.Is<VersionResponseModel>(x => x.Version == "2.2.2" &&
                x.ChangeLogs.Count == 2)), Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_ProductDoesNotContainAnyVersions_NoVersionAvailable()
        {
            // arrange
            _productDaoStub.Products.Add(TestAccount.Product);
            _userDaoStub.Users.Add(TestAccount.User);

            var interactor = CreateInteractor();
            _outputPortMock.Setup(x => x.NoVersionExists(It.IsAny<Guid>()));
            
            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, TestAccount.Product.Id);

            // assert
            _outputPortMock.Verify(m => m.NoVersionExists(It.Is<Guid>(x => x == TestAccount.Product.Id)), Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_ProductDoesNotExist_ProductDoesNotExistOutput()
        {
            // arrange
            _userDaoStub.Users.Add(TestAccount.User);

            var interactor = CreateInteractor();
            _outputPortMock.Setup(x => x.ProductDoesNotExist());
            
            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, TestAccount.Product.Id);

            // assert
            _outputPortMock.Verify(m => m.ProductDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task GetLatestVersion_ProductIdIsEmpty_ArgumentException()
        {
            var interactor = CreateInteractor();

            Func<Task> act = () => interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, Guid.Empty);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetLatestVersion_UserIdIsEmpty_ArgumentException()
        {
            var interactor = CreateInteractor();

            Func<Task> act = () => interactor.ExecuteAsync(_outputPortMock.Object, Guid.Empty, TestAccount.Product.Id);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }
    }
}
