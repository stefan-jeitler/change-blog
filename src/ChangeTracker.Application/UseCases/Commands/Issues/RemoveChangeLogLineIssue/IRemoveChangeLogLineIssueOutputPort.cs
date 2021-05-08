using System;

namespace ChangeTracker.Application.UseCases.Commands.Issues.RemoveChangeLogLineIssue
{
    public interface IRemoveChangeLogLineIssueOutputPort
    {
        void Removed(Guid lineId);
        void Conflict(string reason);
        void ChangeLogLineDoesNotExist();
        void InvalidIssue(string issue);
    }
}