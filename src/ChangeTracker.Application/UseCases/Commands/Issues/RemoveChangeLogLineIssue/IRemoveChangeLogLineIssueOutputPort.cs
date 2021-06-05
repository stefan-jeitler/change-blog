using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.Issues.RemoveChangeLogLineIssue
{
    public interface IRemoveChangeLogLineIssueOutputPort
    {
        void Removed(Guid lineId);
        void Conflict(Conflict conflict);
        void ChangeLogLineDoesNotExist();
        void InvalidIssue(string issue);
    }
}