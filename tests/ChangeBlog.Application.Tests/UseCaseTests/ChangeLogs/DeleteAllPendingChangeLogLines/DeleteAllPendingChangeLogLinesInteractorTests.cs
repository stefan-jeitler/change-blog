using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.ChangeLogs.DeleteAllPendingChangeLogLines;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.ChangeLogs.DeleteAllPendingChangeLogLines;

public class DeleteAllPendingChangeLogLinesInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;

    public DeleteAllPendingChangeLogLinesInteractorTests()
    {
        _fakeChangeLogDao = new FakeChangeLogDao();
    }

    [Fact]
    public async Task DeleteAllPendingLines_HappyPath_SuccessfullyDeleted()
    {
        // arrange
        var pendingLines = Enumerable
            .Range(0, 5)
            .Select(x => new ChangeLogLine(null, TestAccount.Product.Id, ChangeLogText.Parse($"00000{x}"), (uint)x,
                TestAccount.UserId));

        _fakeChangeLogDao.ChangeLogs.AddRange(pendingLines);
        var interactor = new DeleteAllPendingChangeLogLinesInteractor(_fakeChangeLogDao);

        // act
        await interactor.ExecuteAsync(TestAccount.Product.Id);

        // assert
        _fakeChangeLogDao.ChangeLogs.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteAllPendingLines_WithEmptyProductId_ArgumentException()
    {
        var interactor = new DeleteAllPendingChangeLogLinesInteractor(_fakeChangeLogDao);

        var act = () => interactor.ExecuteAsync(Guid.Empty);

        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }
}