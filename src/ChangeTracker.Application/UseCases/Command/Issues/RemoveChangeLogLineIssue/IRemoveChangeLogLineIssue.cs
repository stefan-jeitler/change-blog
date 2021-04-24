using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Command.Issues.SharedModels;

namespace ChangeTracker.Application.UseCases.Command.Issues.RemoveChangeLogLineIssue
{
    public interface IRemoveChangeLogLineIssue
    {
        Task ExecuteAsync(IRemoveChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}