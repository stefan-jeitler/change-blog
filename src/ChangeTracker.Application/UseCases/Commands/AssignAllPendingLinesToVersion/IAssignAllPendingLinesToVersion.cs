using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models;

namespace ChangeTracker.Application.UseCases.Commands.AssignAllPendingLinesToVersion
{
    public interface IAssignAllPendingLinesToVersion
    {
        Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
            VersionIdAssignmentRequestModel requestModel);

        Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
            VersionAssignmentRequestModel requestModel);
    }
}