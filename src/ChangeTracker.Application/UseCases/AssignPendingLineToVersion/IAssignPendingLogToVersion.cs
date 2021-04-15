using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.AssignPendingLineToVersion.Models;

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion
{
    public interface IAssignPendingLogToVersion
    {
        Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionIdAssignmentRequestModel requestModel);
        Task ExecuteAsync(IAssignPendingLineOutputPort output, VersionAssignmentRequestModel requestModel);
    }
}