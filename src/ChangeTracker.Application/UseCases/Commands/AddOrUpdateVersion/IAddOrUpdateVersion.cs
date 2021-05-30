using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion
{
    public interface IAddOrUpdateVersion
    {
        Task ExecuteAsync(IAddOrUpdateVersionOutputPort output, VersionRequestModel requestModel);
    }
}