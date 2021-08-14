using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Queries.GetLabels;
using ChangeBlog.Domain.ChangeLog;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetLabels
{
    public class GetLabelsInteractorTests
    {
        private readonly ChangeLogDaoStub _changeLogDaoStub;

        public GetLabelsInteractorTests()
        {
            _changeLogDaoStub = new ChangeLogDaoStub();
        }

        private GetLabelsInteractor CreateInteractor() => new(_changeLogDaoStub);

        [Fact]
        public async Task GetLabels_HappyPath_Successful()
        {
            // arrange
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            var labels = new[] { Label.Parse("ProxyStrikesBack"), Label.Parse("Security") };
            var line = new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, DateTime.Parse("2021-07-26"),
                labels, Array.Empty<Issue>(), TestAccount.UserId);
            _changeLogDaoStub.ChangeLogs.Add(line);

            var interactor = CreateInteractor();

            // act
            var returnedLabels = await interactor.ExecuteAsync(changeLogLineId);

            // assert
            returnedLabels.Should().HaveCount(2);
            returnedLabels.Should().Contain(x => x == "ProxyStrikesBack");
            returnedLabels.Should().Contain(x => x == "Security");
        }

        [Fact]
        public async Task GetLabels_LineContainsNoIssues_ReturnsEmptyList()
        {
            // arrange
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            var line = new ChangeLogLine(changeLogLineId, null, TestAccount.Product.Id,
                ChangeLogText.Parse("Test line."), 0, DateTime.Parse("2021-07-26"),
                Array.Empty<Label>(), Array.Empty<Issue>(), TestAccount.UserId);
            _changeLogDaoStub.ChangeLogs.Add(line);

            var interactor = CreateInteractor();

            // act
            var returnedLabels = await interactor.ExecuteAsync(changeLogLineId);

            // assert
            returnedLabels.Should().BeEmpty();
        }

        [Fact]
        public async Task GetLabels_NotExistingLine_ReturnsEmptyList()
        {
            // arrange
            var changeLogLineId = Guid.Parse("bf621860-3fa3-40d4-92ac-530cc57a1a98");
            var interactor = CreateInteractor();

            // act
            var returnedLabels = await interactor.ExecuteAsync(changeLogLineId);

            // assert
            returnedLabels.Should().BeEmpty();
        }

        [Fact]
        public void GetLabels_ChangeLogLineIdIsEmpty_ArgumentException()
        {
            var interactor = CreateInteractor();

            Func<Task<IList<string>>> act = () => interactor.ExecuteAsync(Guid.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
