using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.Issues.SharedModels;

namespace ChangeBlog.Application.UseCases.Commands.Issues.AddChangeLogLineIssue
{
    public interface IAddChangeLogLineIssue
    {
        Task ExecuteAsync(IAddChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}
