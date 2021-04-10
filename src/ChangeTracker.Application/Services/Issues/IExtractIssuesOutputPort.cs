using System.Collections.Generic;

namespace ChangeTracker.Application.Services.Issues
{
    public interface IExtractIssuesOutputPort
    {
        void InvalidIssues(List<string> issues);
        void TooManyIssues(int maxIssues);
    }
}