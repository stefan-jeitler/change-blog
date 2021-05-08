using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.Issues.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.Issues.RemoveChangeLogLineIssue
{
    public interface IRemoveChangeLogLineIssue
    {
        Task ExecuteAsync(IRemoveChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}