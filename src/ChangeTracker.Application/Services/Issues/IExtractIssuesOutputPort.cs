using System.Collections.Generic;
using ChangeTracker.Domain.ChangeLog;

namespace ChangeTracker.Application.Services.Issues
{
    public interface IExtractIssuesOutputPort
    {
        void InvalidIssue(string changeLogText, string issue);
        void TooManyIssues(string changeLogText, int maxIssues);
    }
}