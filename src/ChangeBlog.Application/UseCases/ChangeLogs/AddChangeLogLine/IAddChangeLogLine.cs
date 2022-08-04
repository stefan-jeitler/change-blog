using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine.Models;

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine;

public interface IAddChangeLogLine
{
    Task ExecuteAsync(IAddChangeLogLineOutputPort output,
        VersionChangeLogLineRequestModel requestModel);

    Task ExecuteAsync(IAddChangeLogLineOutputPort output,
        VersionIdChangeLogLineRequestModel requestModel);
}