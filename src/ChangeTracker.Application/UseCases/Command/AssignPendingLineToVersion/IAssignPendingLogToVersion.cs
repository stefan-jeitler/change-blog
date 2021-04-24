using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Command.AssignPendingLineToVersion.Models;

namespace ChangeTracker.Application.UseCases.Command.AssignPendingLineToVersion
{
    public interface IAssignPendingLogToVersion
    {
        Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionIdAssignmentRequestModel requestModel);
        Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionAssignmentRequestModel requestModel);
    }
}