using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion.Models;

namespace ChangeTracker.Application.UseCases.Commands.AssignPendingLineToVersion
{
    public interface IAssignPendingLineToVersion
    {
        Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output, VersionIdAssignmentRequestModel requestModel);
        Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output, VersionAssignmentRequestModel requestModel);
    }
}