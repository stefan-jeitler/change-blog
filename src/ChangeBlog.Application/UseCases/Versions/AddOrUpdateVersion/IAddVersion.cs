using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.OutputPorts;

namespace ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion;

public interface IAddVersion
{
    Task ExecuteAsync(IAddVersionOutputPort output, VersionRequestModel versionRequestModel);
}