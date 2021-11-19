using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts;

namespace ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion;

public interface IAddVersion
{
    Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel versionRequestModel);
}