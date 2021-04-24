using System;

namespace ChangeTracker.Application.UseCases.Command.Issues.AddChangeLogLineIssue
{
    public interface IAddChangeLogLineIssueOutputPort
    {
        void Added(Guid lineId);
        void Conflict(string reason);
        void ChangeLogLineDoesNotExist();
        void InvalidIssue(string issue);
        void MaxIssuesReached(int maxIssues);
    }
}