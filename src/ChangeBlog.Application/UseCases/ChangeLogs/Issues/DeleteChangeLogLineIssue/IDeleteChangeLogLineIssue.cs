using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Issues.DeleteChangeLogLineIssue;

public interface IDeleteChangeLogLineIssue
{
    Task ExecuteAsync(IDeleteChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
}