using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetLatestVersion;
using ChangeBlog.Application.UseCases.Queries.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetLatestVersion
{
    public class GetLatestVersionInteractorTests
    {
        private readonly Mock<IGetLatestVersionOutputPort> _outputPortMock;
        private readonly FakeChangeLogDao _fakeChangeLogDao;
        private readonly FakeVersionDao _fakeVersionDao;
        private readonly FakeProductDao _fakeProductDao;
        private readonly FakeUserDao _fakeUserDao;

        public GetLatestVersionInteractorTests()
        {
            _outputPortMock = new Mock<IGetLatestVersionOutputPort>(MockBehavior.Strict);
            _fakeChangeLogDao = new FakeChangeLogDao();
            _fakeVersionDao = new FakeVersionDao();
            _fakeProductDao = new FakeProductDao();
            _fakeUserDao = new FakeUserDao();
        }

        private GetLatestVersionInteractor CreateInteractor() =>
            new(_fakeVersionDao, _fakeChangeLogDao, _fakeProductDao, _fakeUserDao);

        [Fact]
        public async Task GetLatestVersion_HappyPath_Successful()
        {
            // arrange
            var versionId = Guid.Parse("d8d1ab5d-670d-4370-84d5-2a260755e45c");
            
            var latestVersion = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("2.2.2"), OptionalName.Empty,
                null, TestAccount.UserId, DateTime.Parse("2021-08-02"), null);

            var oldVersion = new ClVersion(versionId, TestAccount.Product.Id, ClVersionValue.Parse("1.1.1"), OptionalName.Empty,
                null, TestAccount.UserId, DateTime.Parse("2021-08-01"), null);

            _fakeVersionDao.Versions.Add(latestVersion);
            _fakeVersionDao.Versions.Add(oldVersion);

            var lines = new[]
            {
                new ChangeLogLine(versionId, TestAccount.Product.Id, ChangeLogText.Parse("Test text"), 0, TestAccount.UserId),
                new ChangeLogLine(versionId, TestAccount.Product.Id, ChangeLogText.Parse("Test text 2"), 1, TestAccount.UserId)
            };

            _fakeChangeLogDao.ChangeLogs.AddRange(lines);
            _fakeProductDao.Products.Add(TestAccount.Product);
            _fakeUserDao.Users.Add(TestAccount.User);

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
            _fakeProductDao.Products.Add(TestAccount.Product);
            _fakeUserDao.Users.Add(TestAccount.User);

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
            _fakeUserDao.Users.Add(TestAccount.User);

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

            var act = () => interactor.ExecuteAsync(_outputPortMock.Object, TestAccount.UserId, Guid.Empty);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }

        [Fact]
        public async Task GetLatestVersion_UserIdIsEmpty_ArgumentException()
        {
            var interactor = CreateInteractor();

            var act = () => interactor.ExecuteAsync(_outputPortMock.Object, Guid.Empty, TestAccount.Product.Id);

            await act.Should().ThrowExactlyAsync<ArgumentException>();
        }
    }
}
