using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Issues.AddChangeLogLineIssue;

public class AddChangeLogLineIssueInteractor : IAddChangeLogLineIssue
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IBusinessTransaction _businessTransaction;

    public AddChangeLogLineIssueInteractor(IBusinessTransaction businessTransaction, IChangeLogQueriesDao changeLogQueries,
        IChangeLogCommandsDao changeLogCommands)
    {
        _businessTransaction = businessTransaction ?? throw new ArgumentNullException(nameof(businessTransaction));
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
    }

    public async Task ExecuteAsync(IAddChangeLogLineIssueOutputPort output,
        ChangeLogLineIssueRequestModel requestModel)
    {
        if (!Issue.TryParse(requestModel.Issue, out var issue))
        {
            output.InvalidIssue(requestModel.Issue);
            return;
        }

        _businessTransaction.Start();

        var line = await _changeLogQueries.FindLineAsync(requestModel.ChangeLogLineId);
        if (line.HasNoValue)
        {
            output.ChangeLogLineDoesNotExist();
            return;
        }

        if (line.GetValueOrThrow().AvailableIssuePlaces <= 0)
        {
            output.MaxIssuesReached(ChangeLogLine.MaxIssues);
            return;
        }

        await AddIssueAsync(output, line.GetValueOrThrow(), issue);
    }

    private async Task AddIssueAsync(IAddChangeLogLineIssueOutputPort output, ChangeLogLine line, Issue issue)
    {
        line.AddIssue(issue);

        await _changeLogCommands.UpdateLineAsync(line)
            .Match(Finish, output.Conflict);

        void Finish(ChangeLogLine l)
        {
            _businessTransaction.Commit();
            output.Added(l.Id);
        }
    }
}