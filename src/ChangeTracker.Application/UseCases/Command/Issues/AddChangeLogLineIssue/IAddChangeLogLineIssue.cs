using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Command.Issues.SharedModels;

namespace ChangeTracker.Application.UseCases.Command.Issues.AddChangeLogLineIssue
{
    public interface IAddChangeLogLineIssue
    {
        Task ExecuteAsync(IAddChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}