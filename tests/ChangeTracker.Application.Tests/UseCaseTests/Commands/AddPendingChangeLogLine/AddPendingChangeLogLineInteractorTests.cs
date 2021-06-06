using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddPendingChangeLogLine
{
    public class AddPendingChangeLogLineInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddPendingChangeLogLineOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddPendingChangeLogLineInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IAddPendingChangeLogLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        private AddPendingChangeLogLineInteractor CreateInteractor() => new(_productDaoStub, _changeLogDaoStub,
            _changeLogDaoStub, _unitOfWorkMock.Object);

        [Fact]
        public async Task AddPendingLine_NotExistingProduct_ProductDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, changeLogLine, labels,
                    issues);

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ProductDoesNotExist(It.IsAny<Guid>()));

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ProductDoesNotExist(It.Is<Guid>(x => x == TestAccount.Product.Id)),
                Times.Once);
        }

        [Fact]
        public async Task AddPendingLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            _changeLogDaoStub.Conflict = new ConflictStub();
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Product.Id,
                ChangeLogText.Parse("some-release"),
                0,
                TestAccount.UserId,
                DateTime.Parse("2021-04-09")));

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
        }

        [Fact]
        public async Task AddPendingLine_LineWithSameTextExists_LineWithSameTextExistsOutput()
        {
            // arrange
            const string changeLogLine = "some changes";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            var existingLineWithSameText = new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Product.Id,
                ChangeLogText.Parse("some changes"),
                0,
                TestAccount.UserId,
                DateTime.Parse("2021-04-09"));
            _changeLogDaoStub.ChangeLogs.Add(existingLineWithSameText);

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.LinesWithSameTextsAreNotAllowed(It.IsAny<Guid>(), It.IsAny<string>()));

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(
                m => m.LinesWithSameTextsAreNotAllowed(It.Is<Guid>(x => x == existingLineWithSameText.Id),
                    It.Is<string>(x => x == changeLogLine)),
                Times.Once);
        }

        [Fact]
        public async Task AddPendingLine_MaxLinesReached_TooManyLinesOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100)
                .Select(x =>
                    new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x,
                        TestAccount.UserId)));

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.TooManyLines(It.IsAny<int>()));

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.TooManyLines(It.Is<int>(x => x == ChangeLogs.MaxLines)),
                Times.Once);
        }

        [Fact]
        public async Task AddPendingLine_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var lineRequestModel =
                new PendingChangeLogLineRequestModel(TestAccount.UserId, TestAccount.Product.Id, changeLogLine, labels,
                    issues);

            _productDaoStub.Products.Add(new Product(TestAccount.Product.Id, TestAccount.Id, TestAccount.Product.Name,
                TestAccount.CustomVersioningScheme, TestAccount.Product.LanguageCode, TestAccount.UserId,
                TestAccount.CreationDate, null));

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Product.Id,
                ChangeLogText.Parse("some-release"),
                0,
                TestAccount.UserId,
                DateTime.Parse("2021-04-09")));

            var addPendingLineInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addPendingLineInteractor.ExecuteAsync(_outputPortMock.Object, lineRequestModel);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }
    }
}