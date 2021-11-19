using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion.Models;

namespace ChangeBlog.Application.UseCases.Commands.AssignPendingLineToVersion;

public interface IAssignPendingLineToVersion
{
    Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output, VersionIdAssignmentRequestModel requestModel);
    Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output, VersionAssignmentRequestModel requestModel);
}