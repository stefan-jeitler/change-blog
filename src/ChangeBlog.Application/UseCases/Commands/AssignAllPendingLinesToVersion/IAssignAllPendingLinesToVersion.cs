using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion.Models;

namespace ChangeBlog.Application.UseCases.Commands.AssignAllPendingLinesToVersion;

public interface IAssignAllPendingLinesToVersion
{
    Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
        VersionIdAssignmentRequestModel requestModel);

    Task ExecuteAsync(IAssignAllPendingLinesToVersionOutputPort output,
        VersionAssignmentRequestModel requestModel);
}