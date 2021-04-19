using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Issues.SharedModels;

namespace ChangeTracker.Application.UseCases.Issues.RemoveChangeLogLineIssue
{
    public interface IRemoveChangeLogLineIssue
    {
        Task ExecuteAsync(IRemoveChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}