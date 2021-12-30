using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ChangeLog;
using ChangeBlog.Application.UseCases.Commands.Issues.SharedModels;
using ChangeBlog.Domain.ChangeLog;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;

public class DeleteChangeLogLineIssueInteractor : IDeleteChangeLogLineIssue
{
    private readonly IChangeLogCommandsDao _changeLogCommands;
    private readonly IChangeLogQueriesDao _changeLogQueries;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteChangeLogLineIssueInteractor(IChangeLogQueriesDao changeLogQueries,
        IChangeLogCommandsDao changeLogCommands, IUnitOfWork unitOfWork)
    {
        _changeLogQueries = changeLogQueries ?? throw new ArgumentNullException(nameof(changeLogQueries));
        _changeLogCommands = changeLogCommands ?? throw new ArgumentNullException(nameof(changeLogCommands));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task ExecuteAsync(IDeleteChangeLogLineIssueOutputPort output,
        ChangeLogLineIssueRequestModel requestModel)
    {
        if (!Issue.TryParse(requestModel.Issue, out var issue))
        {
            output.InvalidIssue(requestModel.Issue);
            return;
        }

        _unitOfWork.Start();

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
            _unitOfWork.Commit();
            output.Removed(l.Id);
        }
    }
}