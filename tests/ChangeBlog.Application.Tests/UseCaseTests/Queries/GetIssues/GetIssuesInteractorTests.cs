using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetIssues;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetIssues;

public class GetIssuesInteractorTests
{
    private readonly FakeChangeLogDao _fakeChangeLogDao;

    public GetIssuesInteractorTests()
    {
        _fakeChangeLogDao = new FakeChangeLogDao();
    }

    private GetIssuesInteractor CreateInteractor() => new(_fakeChangeLogDao);

    [Fact]
    public async Task GetIssues_HappyPath_Successful()
    {
        // arrange
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
        var issues = new[] { Issue.Parse("#123"), Issue.Parse("#456") };
        var line = new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Test line."), 0, DateTime.Parse("2021-07-26"),
            Array.Empty<Label>(), issues, TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(line);

        var interactor = CreateInteractor();

        // act
        var returnedIssues = await interactor.ExecuteAsync(changeLogLineId);

        // assert
        returnedIssues.Should().HaveCount(2);
        returnedIssues.Should().Contain(x => x == "#123");
        returnedIssues.Should().Contain(x => x == "#456");
    }

    [Fact]
    public async Task GetIssues_LineContainsNoIssues_ReturnsEmptyList()
    {
        // arrange
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
        var line = new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
            ChangeLogText.Parse("Test line."), 0, DateTime.Parse("2021-07-26"),
            Array.Empty<Label>(), Array.Empty<Issue>(), TestAccount.UserId);
        _fakeChangeLogDao.ChangeLogs.Add(line);

        var interactor = CreateInteractor();

        // act
        var returnedIssues = await interactor.ExecuteAsync(changeLogLineId);

        // assert
        returnedIssues.Should().BeEmpty();
    }

    [Fact]
    public async Task GetIssues_NotExistingLine_ReturnsEmptyList()
    {
        // arrange
        var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
        var interactor = CreateInteractor();

        // act
        var returnedIssues = await interactor.ExecuteAsync(changeLogLineId);

        // assert
        returnedIssues.Should().BeEmpty();
    }

    [Fact]
    public async Task GetIssues_ChangeLogLineIdIsEmpty_ArgumentException()
    {
        var interactor = CreateInteractor();

        var act = () => interactor.ExecuteAsync(Guid.Empty);

        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }
}