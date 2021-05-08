using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AddChangeLogLine.Models;

namespace ChangeTracker.Application.UseCases.Commands.AddChangeLogLine
{
    public interface IAddChangeLogLine
    {
        Task ExecuteAsync(IAddLineOutputPort output, VersionChangeLogLineRequestModelRequestModel requestModelRequestModel);
        Task ExecuteAsync(IAddLineOutputPort output, VersionIdChangeLogLineRequestModelRequestModel requestModelRequestModel);
    }
}