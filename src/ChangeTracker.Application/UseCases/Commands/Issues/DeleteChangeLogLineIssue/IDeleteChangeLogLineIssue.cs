using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.Issues.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.Issues.DeleteChangeLogLineIssue
{
    public interface IDeleteChangeLogLineIssue
    {
        Task ExecuteAsync(IDeleteChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}