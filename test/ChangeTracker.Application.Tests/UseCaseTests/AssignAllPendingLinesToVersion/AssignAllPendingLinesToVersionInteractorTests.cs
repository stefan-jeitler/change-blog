using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion;
using ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion.Models;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AssignAllPendingLinesToVersion
{
    public class AssignAllPendingLinesToVersionInteractorTests
    {
        private readonly Mock<IAssignAllPendingLinesToVersionOutputPort> _outputPortMock;
        private readonly Mock<IUnitOfWork> _unitOfWork;
        private readonly VersionDaoStub _versionDaoStub;
        private readonly ChangeLogDaoStub _changeLogDaoStub;

        public AssignAllPendingLinesToVersionInteractorTests()
        {
            _outputPortMock = new Mock<IAssignAllPendingLinesToVersionOutputPort>(MockBehavior.Strict);
            _versionDaoStub = new VersionDaoStub();
            _unitOfWork = new Mock<IUnitOfWork>();
            _changeLogDaoStub = new ChangeLogDaoStub();
        }

        private AssignAllPendingLinesToVersionInteractor CreateInteractor() => new(_versionDaoStub, _unitOfWork.Object, _changeLogDaoStub, _changeLogDaoStub);

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
    }
}
