using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.AddChangeLogLine.Models;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public interface IAddChangeLogLine
    {
        Task ExecuteAsync(IAddLineOutputPort output, VersionChangeLogLineRequestModel requestModel);
        Task ExecuteAsync(IAddLineOutputPort output, VersionIdChangeLogLineRequestModel requestModel);
    }
}