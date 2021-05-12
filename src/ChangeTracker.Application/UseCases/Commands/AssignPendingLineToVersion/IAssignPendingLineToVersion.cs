using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion.Models;

namespace ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion
{
    public interface IAssignPendingLineToVersion
    {
        Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionIdAssignmentRequestModel requestModel);
        Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionAssignmentRequestModel requestModel);
    }
}