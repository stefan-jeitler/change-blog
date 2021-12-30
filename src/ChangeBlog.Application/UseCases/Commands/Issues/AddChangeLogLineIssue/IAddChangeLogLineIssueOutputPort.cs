using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.Issues.AddChangeLogLineIssue;

public interface IAddChangeLogLineIssueOutputPort
{
    void Added(Guid changeLogLineId);
    void Conflict(Conflict conflict);
    void ChangeLogLineDoesNotExist();
    void InvalidIssue(string issue);
    void MaxIssuesReached(int maxIssues);
}