using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.Issues.SharedModels;

namespace ChangeBlog.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue;

public interface IDeleteChangeLogLineIssue
{
    Task ExecuteAsync(IDeleteChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
}