using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.Issues.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.Issues.AddChangeLogLineIssue
{
    public interface IAddChangeLogLineIssue
    {
        Task ExecuteAsync(IAddChangeLogLineIssueOutputPort output, ChangeLogLineIssueRequestModel requestModel);
    }
}