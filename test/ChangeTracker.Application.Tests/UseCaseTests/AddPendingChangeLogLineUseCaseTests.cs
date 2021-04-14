using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.ChangeLog;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.AddPendingChangeLogLine;
using ChangeTracker.Domain;
using ChangeTracker.Domain.ChangeLog;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests
{
    public class AddPendingChangeLogLineUseCaseTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;
        private readonly Mock<IAddPendingChangeLogLineOutputPort> _outputPortMock;
        private readonly ProjectDaoStub _projectDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AddPendingChangeLogLineUseCaseTests()
        {
            _projectDaoStub = new ProjectDaoStub();
            _changeLogDaoStub = new ChangeLogDaoStub();
            _outputPortMock = new Mock<IAddPendingChangeLogLineOutputPort>(MockBehavior.Strict);
            _unitOfWorkMock = new Mock<IUnitOfWork>(MockBehavior.Strict);
        }

        [Fact]
        public async Task AddChangeLogLine_NotExistingProject_ProjectDoesNotExistOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new PendingChangeLogLineDto(TestAccount.Project.Id, changeLogLine, labels, issues);

            var addPendingChangeLogLineUseCase =
                new AddPendingChangeLogLineUseCase(_projectDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object,
                    new ChangeLogLineParsingService(_changeLogDaoStub));

            _outputPortMock.Setup(m => m.ProjectDoesNotExist());

            // act
            await addPendingChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.ProjectDoesNotExist(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ConflictWhileSaving_ConflictOutput()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new PendingChangeLogLineDto(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoStub.ProduceConflict = true;
            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addPendingChangeLogLineUseCase =
                new AddPendingChangeLogLineUseCase(_projectDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object,
                    new ChangeLogLineParsingService(_changeLogDaoStub));

            _outputPortMock.Setup(m => m.Conflict(It.IsAny<string>()));
            _unitOfWorkMock.Setup(m => m.Start());

            // act
            await addPendingChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.Conflict(It.IsAny<string>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
        }

        [Fact]
        public async Task AddChangeLogLine_ValidLine_CreatedOutputAndCommittedUow()
        {
            // arrange
            const string changeLogLine = "Some Bug fixed";
            var labels = new List<string> {"Bugfix", "ProxyIssue"};
            var issues = new List<string> {"#1234", "#12345"};
            var changeLogLineDto =
                new PendingChangeLogLineDto(TestAccount.Project.Id, changeLogLine, labels, issues);

            _projectDaoStub.Projects.Add(new Project(TestAccount.Project.Id, TestAccount.Id, TestAccount.Project.Name,
                TestAccount.CustomVersioningScheme, TestAccount.CreationDate, null));

            _changeLogDaoStub.ChangeLogs.Add(new ChangeLogLine(Guid.NewGuid(),
                null,
                TestAccount.Project.Id,
                ChangeLogText.Parse("some-release"),
                0,
                DateTime.Parse("2021-04-09")));

            var addPendingChangeLogLineUseCase =
                new AddPendingChangeLogLineUseCase(_projectDaoStub, _changeLogDaoStub, _unitOfWorkMock.Object,
                    new ChangeLogLineParsingService(_changeLogDaoStub));

            _outputPortMock.Setup(m => m.Created(It.IsAny<Guid>()));
            _unitOfWorkMock.Setup(m => m.Start());
            _unitOfWorkMock.Setup(m => m.Commit());

            // act
            await addPendingChangeLogLineUseCase.ExecuteAsync(_outputPortMock.Object, changeLogLineDto);

            // assert
            _outputPortMock.Verify(m => m.Created(It.IsAny<Guid>()), Times.Once);
            _unitOfWorkMock.Verify(m => m.Start(), Times.Once);
            _unitOfWorkMock.Verify(m => m.Commit(), Times.Once);
        }
    }
}