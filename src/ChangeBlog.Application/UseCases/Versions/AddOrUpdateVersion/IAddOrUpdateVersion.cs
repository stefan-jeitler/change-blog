using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion.OutputPorts;

namespace ChangeBlog.Application.UseCases.Versions.AddOrUpdateVersion;

public interface IAddOrUpdateVersion
{
    Task ExecuteAsync(IAddOrUpdateVersionOutputPort output, VersionRequestModel requestModel);
}