using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Issues.DeleteChangeLogLineIssue;

public interface IDeleteChangeLogLineIssueOutputPort
{
    void Removed(Guid changeLogLineId);
    void Conflict(Conflict conflict);
    void ChangeLogLineDoesNotExist();
    void InvalidIssue(string issue);
}