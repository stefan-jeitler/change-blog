using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts;

namespace ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion
{
    public interface IAddVersion
    {
        Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel versionRequestModel);
    }
}