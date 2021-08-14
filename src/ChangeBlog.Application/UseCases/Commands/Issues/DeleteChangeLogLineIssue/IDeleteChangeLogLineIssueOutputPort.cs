using System;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue
{
    public interface IDeleteChangeLogLineIssueOutputPort
    {
        void Removed(Guid changeLogLineId);
        void Conflict(Conflict conflict);
        void ChangeLogLineDoesNotExist();
        void InvalidIssue(string issue);
    }
}
