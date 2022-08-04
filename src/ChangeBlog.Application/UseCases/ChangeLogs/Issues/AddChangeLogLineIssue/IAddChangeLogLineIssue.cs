using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.ChangeLogs.Issues.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Issues.AddChangeLogLineIssue;

public interface IAddChangeLogLineIssue
{
    Task ExecuteAsync(IAddChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
}