using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Versions.AssignAllPendingLinesToVersion.Models;

namespace ChangeBlog.Application.UseCases.Versions.AssignAllPendingLinesToVersion;

public interface IAssignAllPendingLinesToVersion
{
    Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
        VersionIdAssignmentRequestModel requestModel);

    Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
        VersionAssignmentRequestModel requestModel);
}