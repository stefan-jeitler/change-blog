using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.AddChangeLogLine.Models;

namespace ChangeBlog.Application.UseCases.Commands.AddChangeLogLine;

public interface IAddChangeLogLine
{
    Task ExecuteAsync(IAddChangeLogLineOutputPort output,
        VersionChangeLogLineRequestModel requestModel);

    Task ExecuteAsync(IAddChangeLogLineOutputPort output,
        VersionIdChangeLogLineRequestModel requestModel);
}