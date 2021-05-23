using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion.Models;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AssignPendingLineToVersion
{
    public class AssignPendingLineToVersionInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAssignPendingLineOutputPort> _outputPortMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;

        public AssignPendingLineToVersionInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IAssignPendingLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>();
        }

        private AssignPendingLineToVersionInteractor CreateInteractor()
        {
            return new(_versionDaoStub, _changeLogDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task AssignPendingLineByVersionId_HappyPath_AssignedAndUowCommitted()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, TestAccount.UserId);
            _versionDaoStub.Versions.Add(clVersion);

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some new changes"), 0, TestAccount.UserId, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var assignmentRequestModel =
                new VersionIdAssignmentRequestModel(clVersion.Id, line.Id);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Assigned(It.IsAny<Guid>(), It.IsAny<Guid>()));

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _changeLogDaoStub.ChangeLogs.Should().ContainSingle(x => x.VersionId == clVersion.Id);
            _outputPortMock.Verify(m => m.Assigned(It.Is<Guid>(x => x == clVersion.Id), It.Is<Guid>(x => x == line.Id)),
                Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLineByVersionValue_InvalidVersionFormat_InvalidVersionFormatOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some new changes"), 0, TestAccount.UserId, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var assignmentRequestModel = new VersionAssignmentRequestModel(TestAccount.Product.Id, "1. .2", line.Id);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == "1. .2")), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLineByVersionValue_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, TestAccount.UserId);
            _versionDaoStub.Versions.Add(clVersion);

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some new changes"), 0, TestAccount.UserId, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var assignmentRequestModel = new VersionAssignmentRequestModel(TestAccount.Product.Id, "1.3", line.Id);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some new changes"), 0, TestAccount.UserId, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var assignmentRequestModel =
                new VersionIdAssignmentRequestModel(versionId, line.Id);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_NotExistingChangeLogLine_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, TestAccount.UserId);
            _versionDaoStub.Versions.Add(clVersion);

            var lineId = Guid.Parse("2b4b147a-9ebd-4350-a45b-aaae5d8d63de");

            var assignmentRequestModel =
                new VersionIdAssignmentRequestModel(clVersion.Id, lineId);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineDoesNotExist());

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_TooManyLinesAssigned_ChangeLogLineDoesNotExistOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, TestAccount.UserId);
            _versionDaoStub.Versions.Add(clVersion);

            var lineId = Guid.Parse("2b4b147a-9ebd-4350-a45b-aaae5d8d63de");

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 100).Select(x =>
                new ChangeLogLine(clVersion.Id, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x, TestAccount.UserId)));

            var assignmentRequestModel =
                new VersionIdAssignmentRequestModel(clVersion.Id, lineId);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.MaxChangeLogLinesReached(It.IsAny<int>()));

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _outputPortMock.Verify(
                m => m.MaxChangeLogLinesReached(It.Is<int>(x => x == ChangeLogs.MaxLines)),
                Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_VersionContainsALineWithSameText_LineWithSameTextAlreadyExistsOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, TestAccount.UserId);
            _versionDaoStub.Versions.Add(clVersion);

            var pendingLine = new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse("00000"), 0, TestAccount.UserId);
            _changeLogDaoStub.ChangeLogs.Add(pendingLine);

            _changeLogDaoStub.ChangeLogs.AddRange(Enumerable.Range(0, 1).Select(x =>
                new ChangeLogLine(clVersion.Id, TestAccount.Product.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x, TestAccount.UserId)));

            var assignmentRequestModel =
                new VersionIdAssignmentRequestModel(clVersion.Id, pendingLine.Id);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.LineWithSameTextAlreadyExists(It.IsAny<string>()));

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _outputPortMock.Verify(m => m.LineWithSameTextAlreadyExists(It.Is<string>(x => x == "00000")), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_ExistingLineIsNotPending_ChangeLogLineIsNotPending()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, TestAccount.UserId);
            _versionDaoStub.Versions.Add(clVersion);

            var line = new ChangeLogLine(Guid.NewGuid(), clVersion.Id, TestAccount.Product.Id,
                ChangeLogText.Parse("Some new changes"), 0, TestAccount.UserId, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            var assignmentRequestModel =
                new VersionIdAssignmentRequestModel(clVersion.Id, line.Id);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.ChangeLogLineIsNotPending(It.IsAny<Guid>()));

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _outputPortMock.Verify(m => m.ChangeLogLineIsNotPending(It.Is<Guid>(x => x == line.Id)), Times.Once);
        }

        [Fact]
        public async Task AssignPendingLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            _productDaoStub.Products.Add(new Product(TestAccount.Id,
                Name.Parse("Test Product"),
                TestAccount.CustomVersioningScheme,
                TestAccount.UserId,
                TestAccount.CreationDate));

            var clVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2"), OptionalName.Empty, TestAccount.UserId);
            _versionDaoStub.Versions.Add(clVersion);

            var line = new ChangeLogLine(Guid.NewGuid(), null, TestAccount.Product.Id,
                ChangeLogText.Parse("Some new changes"), 0, TestAccount.UserId, DateTime.Parse("2021-04-12"));
            _changeLogDaoStub.ChangeLogs.Add(line);

            _changeLogDaoStub.ProduceConflict = true;

            var assignmentRequestModel =
                new VersionIdAssignmentRequestModel(clVersion.Id, line.Id);
            var assignToVersionInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await assignToVersionInteractor.ExecuteAsync(_outputPortMock.Object, assignmentRequestModel);

            // assert
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Never);
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }
    }
}