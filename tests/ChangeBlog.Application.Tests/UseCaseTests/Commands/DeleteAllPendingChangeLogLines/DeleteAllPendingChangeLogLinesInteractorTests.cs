using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Commands.DeleteAllPendingChangeLogLines;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Commands.DeleteAllPendingChangeLogLines
{
    public class DeleteAllPendingChangeLogLinesInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;

        public DeleteAllPendingChangeLogLinesInteractorTests()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
        }

        [Fact]
        public async Task DeleteAllPendingLines_HappyPath_SuccessfullyDeleted()
        {
            // arrange
            var pendingLines = Enumerable
                .Range(0, 5)
                .Select(x => new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse($"00000{x}"), (uint) x,
                    TestAccount.UserId));

            _changeLogDaoStub.ChangeLogs.AddRange(pendingLines);
            var interactor = new DeleteAllPendingChangeLogLinesInteractor(_changeLogDaoStub);

            // act
            await interactor.ExecuteAsync(TestAccount.Product.Id);

            // assert
            _changeLogDaoStub.ChangeLogs.Should().BeEmpty();
        }

        [Fact]
        public void DeleteAllPendingLines_WithEmptyProductId_ArgumentException()
        {
            var interactor = new DeleteAllPendingChangeLogLinesInteractor(_changeLogDaoStub);

            Func<Task> act = () => interactor.ExecuteAsync(Guid.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
