using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue
{
    public interface IDeleteChangeLogLineIssueOutputPort
    {
        void Removed(Guid changeLogLineId);
        void Conflict(Conflict conflict);
        void ChangeLogLineDoesNotExist();
        void InvalidIssue(string issue);
    }
}