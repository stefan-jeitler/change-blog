using System.Threading.Tasks;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.Models;
using ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.OutputPorts;

namespace ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion;

public interface IAddOrUpdateVersion
{
    Task ExecuteAsync(IAddOrUpdateVersionOutputPort output, VersionRequestModel requestModel);
}