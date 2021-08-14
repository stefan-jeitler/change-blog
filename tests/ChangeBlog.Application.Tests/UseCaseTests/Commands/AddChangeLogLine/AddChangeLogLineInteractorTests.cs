using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess;
using ChangeBlog.Application.DataAccess.Conflicts;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.AddChangeLogLine;
using ChangeBlog.Application.UseCases.Commands.AddChangeLogLine.Models;
using ChangeBlog.Domain;
using ChangeBlog.Domain.ChangeLog;
using ChangeBlog.Domain.Common;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.AddChangeLogLine
{
    public class AddChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddChangeLogLineOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AddChangeLogLineInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IAddChangeLogLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        private AddChangeLogLineInteractor CreateInteractor() =>
            new(_changeLogDaoStub, _changeLogDaoStub,
                _unitOfWorkMock.Object, _versionDaoStub);

        [Fact]
        public async Task AddChangeLogLine_InvalidVersion_InvalidVersionFormatOutput()
        {
            // arrange
            const string changeLogLine = "Bug fixed.";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new VersionChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1. .3",
                    changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .3")), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new VersionChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2",
                    changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty,
                TestAccount.UserId);
            _versionDaoStub.Versions.Add(version);

            _changeLogDaoStub.Conflict = new VersionDeletedConflict(version.Id);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_LineWithSameTextExists_()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new VersionChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2",
                    changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var version = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty,
                TestAccount.UserId);
            _versionDaoStub.Versions.Add(version);

            var existingLine = new ChangeLogLine(version.Id, TestAccount.Product.Id,
                ChangeLogText.Parse(changeLogLine), 0, TestAccount.UserId);
            _changeLogDaoStub.ChangeLogs.Add(existingLine);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.LineWithSameTextAlreadyExists(It.IsAny<Guid>(), It.IsAny<string>()));

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(
                m => m.LineWithSameTextAlreadyExists(It.Is<Guid>(x => x == existingLine.Id),
                    It.Is<string>(x => x == changeLogLine)),
                Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new VersionChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2",
                    changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLineByVersionId_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var notExistingVersionId = Guid.Parse("e2eeaad4-dc62-4bb5-8581-f3bf1702255a");
            var changeLogLineRequestModel =
                new VersionIdChangeLogLineRequestModel(TestAccount.UserId, notExistingVersionId,
                    changeLogLine, labels, issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }


        [Fact]
        public async Task AddChangeLogLineByVersionId_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var changeLogLineRequestModel =
                new VersionIdChangeLogLineRequestModel(TestAccount.UserId, versionId, changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var version = new ClVersion(versionId,
                TestAccount.Product.Id,
                ClVersionValue.Parse("1.2"),
                OptionalName.Empty,
                null,
                TestAccount.UserId,
                DateTime.Parse("2021-04-09"),
                null);
            _versionDaoStub.Versions.Add(version);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }


        [Fact]
        public async Task AddChangeLogLine_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new VersionChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2",
                    changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Product.Id,
                ClVersionValue.Parse("1.2"),
                OptionalName.Empty,
                null,
                TestAccount.UserId,
                DateTime.Parse("2021-04-09"),
                null);
            _versionDaoStub.Versions.Add(version);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_MaxLinesReached_TooManyLinesOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new VersionChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2",
                    changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, null, TestAccount.UserId,
                DateTime.Parse("2021-04-09"), null);
            _versionDaoStub.Versions.Add(version);

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100)
                .Select(x =>
                    new ChangeLogLine(versionId, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x,
                        TestAccount.UserId)));

            var addLineInteractor = CreateInteractor();
            _outputPortMock.Setup(m => m.TooManyLines(It.IsAny<int>()));

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.TooManyLines(It.Is<int>(x => x == ChangeLogs.MaxLines)),
                Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ValidLine_ProperlySaved()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineRequestModel =
                new VersionChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2",
                    changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var version = new ClVersion(versionId,
                TestAccount.Product.Id,
                ClVersionValue.Parse("1.2"),
                OptionalName.Empty,
                null,
                TestAccount.UserId,
                DateTime.Parse("2021-04-09"),
                null);
            _versionDaoStub.Versions.Add(version);

            var addLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));

            // act
            await addLineInteractor.ExecuteAsync(_outputPortMock.Object, changeLogLineRequestModel);

            // assert
            var savedLine = _changeLogDaoStub.ChangeLogs.Single(x =>
                x.ProductId == TestAccount.Product.Id && x.VersionId!.Value == versionId);

            savedLine.Position.Should().Be(0);
            savedLine.IsPending.Should().BeFalse();
            savedLine.DeletedAt.Should().BeNull();
            savedLine.Issues.Count.Should().Be(2);
            savedLine.Labels.Count.Should().Be(1);
        }
    }
}
