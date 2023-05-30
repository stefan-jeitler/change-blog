using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Issues.DeleteChangeLogLineIssue;

public class DeleteChangeLogLineIssueInteractor : IDeleteChangeLogLineIssue
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IBusinessTransaction _businessTransaction;

    public DeleteChangeLogLineIssueInteractor(IChangeLogQueriesDao changeLogQueries,
        IChangeLogCommandsDao changeLogCommands, IBusinessTransaction businessTransaction)
    {
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        _businessTransaction = businessTransaction ?? throw new ArgumentNullException(nameof(businessTransaction));
    }

    public async Task ExecuteAsync(IDeleteChangeLogLineIssueOutputPort output,
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

        await RemoveIssueAsync(output, line.GetValueOrThrow(), issue);
    }

    private async Task RemoveIssueAsync(IDeleteChangeLogLineIssueOutputPort output, ChangeLogLine line, Issue issue)
    {
        line.RemoveIssue(issue);

        await _changeLogCommands.UpdateLineAsync(line)
            .Match(Finish, output.Conflict);

        void Finish(ChangeLogLine l)
        {
            _businessTransaction.Commit();
            output.Removed(l.Id);
        }
    }
}