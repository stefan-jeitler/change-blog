using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Issues.SharedModels;

namespace ChangeTracker.Application.UseCases.Issues.AddChangeLogLineIssue
{
    public interface IAddChangeLogLineIssue
    {
        Task ExecuteAsync(IAddChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}