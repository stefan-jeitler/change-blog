using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AddChangeLogLine.Models;

namespace ChangeTracker.Application.UseCases.Commands.AddChangeLogLine
{
    public interface IAddChangeLogLine
    {
        Task ExecuteAsync(IAddChangeLogLineOutputPort output,
            VersionChangeLogLineRequestModel requestModel);

        Task ExecuteAsync(IAddChangeLogLineOutputPort output,
            VersionIdChangeLogLineRequestModel requestModel);
    }
}