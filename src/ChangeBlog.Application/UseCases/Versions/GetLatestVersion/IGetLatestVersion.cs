using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Versions.GetLatestVersion;

public interface IGetLatestVersion
{
    Task ExecuteAsync(IGetLatestVersionOutputPort output, Guid userId, Guid productId);
}