using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Command.AssignAllPendingLinesToVersion;
using ChangeTracker.Application.UseCases.Command.AssignAllPendingLinesToVersion.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AssignAllPendingLinesToVersion
{
    public class AssignAllPendingLinesToVersionInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAssignAllPendingLinesToVersionOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly VersionDaoStub _versionDaoStub;

        public AssignAllPendingLinesToVersionInteractorTests()
        {
            _outputPortMock = new Mock<IAssignAllPendingLinesToVersionOutputPort>(MockBehavior.Strict);
            _versionDaoStub = new VersionDaoStub();
            _unitOfWork = new Mock<IUnitOfWork>();
            _changeLogDaoStub = new ChangeLogDaoStub();
        }

        private AssignAllPendingLinesToVersionInteractor CreateInteractor() => new(_versionDaoStub, _unitOfWork.Object,
            _changeLogDaoStub, _changeLogDaoStub);

        [Fact]
        public async Task AssignAllPendingLines_HappyPath_AssignAndUowCommitted()
        {
            // arrange
            var requestModel = new VersionAssignmentRequestModel(TestAccount.Project.Id, "1.2.3");
            var assignAllPendingLinesInteractor = CreateInteractor();

            var pendingLines = Enumerable.Range(0, 3)
                .Select(x => new ChangeLogLine(null, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x));
            _changeLogDaoStub.ChangeLogs.AddRange(pendingLines);

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            _outputPortMock.Setup(m => m.Assigned(It.IsAny<Guid>()));

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Assigned(It.Is<Guid>(x => x == clVersion.Id)), Times.Once);
            _unitOfWork.Verify(m => m.Start(), Times.Once);
            _unitOfWork.Verify(m => m.Commit(), Times.Once);
            _changeLogDaoStub.ChangeLogs.Should().HaveCount(3);
            _changeLogDaoStub.ChangeLogs.All(x => x.VersionId == clVersion.Id).Should().BeTrue();
        }

        [Fact]
        public async Task AssignAllPendingLinesByVersionId_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            var versionId = Guid.Parse("1d7831d5-32fb-437f-a9d5-bf5a7dd34b10");
            var requestModel = new VersionIdAssignmentRequestModel(TestAccount.Project.Id, versionId);
            var assignAllPendingLinesInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignAllPendingLines_InvalidVersionFormat_InvalidVersionFormatOutput()
        {
            // arrange
            var requestModel = new VersionAssignmentRequestModel(TestAccount.Project.Id, "1. .3");
            var assignAllPendingLinesInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.InvalidVersionFormat(It.IsAny<string>()));

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.InvalidVersionFormat(It.Is<string>(x => x == requestModel.Version)),
                Times.Once);
        }

        [Fact]
        public async Task AssignAllPendingLines_NotExistingVersion_VersionDoesNotExistOutput()
        {
            // arrange
            var requestModel = new VersionAssignmentRequestModel(TestAccount.Project.Id, "1.2.3");
            var assignAllPendingLinesInteractor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionDoesNotExist());

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AssignAllPendingLines_NoPendingLines_NoPendingChangeLogLinesOutput()
        {
            // arrange
            var requestModel = new VersionAssignmentRequestModel(TestAccount.Project.Id, "1.2.3");
            var assignAllPendingLinesInteractor = CreateInteractor();

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            _outputPortMock.Setup(m => m.NoPendingChangeLogLines());

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.NoPendingChangeLogLines(), Times.Once);
        }

        [Fact]
        public async Task AssignAllPendingLines_NotEnoughLinePlacesAvailable_TooManyLinesToAddOutput()
        {
            // arrange
            var requestModel = new VersionAssignmentRequestModel(TestAccount.Project.Id, "1.2.3");
            var assignAllPendingLinesInteractor = CreateInteractor();

            var pendingLines = Enumerable.Range(0, 60)
                .Select(x => new ChangeLogLine(null, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x));
            _changeLogDaoStub.ChangeLogs.AddRange(pendingLines);

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            var assignedLines = Enumerable.Range(0, 60)
                .Select(x =>
                    new ChangeLogLine(clVersion.Id, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x));
            _changeLogDaoStub.ChangeLogs.AddRange(assignedLines);

            _outputPortMock.Setup(m => m.TooManyLinesToAdd(It.IsAny<uint>()));

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.TooManyLinesToAdd(It.Is<uint>(x => x == 40)), Times.Once);
        }

        [Fact]
        public async Task
            AssignAllPendingLines_PendingLinesContainsTextsThatExistsAlreadyInVersion_LinesWithSameTextAlreadyExistsOutput()
        {
            // arrange
            var requestModel = new VersionAssignmentRequestModel(TestAccount.Project.Id, "1.2.3");
            var assignAllPendingLinesInteractor = CreateInteractor();

            var pendingLines = Enumerable.Range(0, 10)
                .Select(x => new ChangeLogLine(null, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x));
            _changeLogDaoStub.ChangeLogs.AddRange(pendingLines);

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            var assignedLine = new ChangeLogLine(clVersion.Id, TestAccount.Project.Id, ChangeLogText.Parse("00000"), 0);
            _changeLogDaoStub.ChangeLogs.Add(assignedLine);

            _outputPortMock.Setup(m => m.LineWithSameTextAlreadyExists(It.IsAny<List<string>>()));

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(
                m => m.LineWithSameTextAlreadyExists(It.Is<List<string>>(x => x.Single() == "00000")), Times.Once);
        }

        [Fact]
        public async Task AssignAllPendingLines_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            var requestModel = new VersionAssignmentRequestModel(TestAccount.Project.Id, "1.2.3");
            var assignAllPendingLinesInteractor = CreateInteractor();

            var pendingLines = Enumerable.Range(0, 60)
                .Select(x => new ChangeLogLine(null, TestAccount.Project.Id, ChangeLogText.Parse($"{x:D5}"), (uint) x));
            _changeLogDaoStub.ChangeLogs.AddRange(pendingLines);

            var clVersion = new ClVersion(TestAccount.Project.Id, ClVersionValue.Parse("1.2.3"));
            _versionDaoStub.Versions.Add(clVersion);

            _changeLogDaoStub.ProduceConflict = true;

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));

            // act
            await assignAllPendingLinesInteractor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
        }
    }
}