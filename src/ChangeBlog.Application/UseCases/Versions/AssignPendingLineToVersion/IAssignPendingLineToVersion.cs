using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Versions.AssignPendingLineToVersion.Models;

namespace ChangeBlog.Application.UseCases.Versions.AssignPendingLineToVersion;

public interface IAssignPendingLineToVersion
{
    Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output, VersionIdAssignmentRequestModel requestModel);
    Task ExecuteAsync(IAssignPendingLineToVersionOutputPort output, VersionAssignmentRequestModel requestModel);
}