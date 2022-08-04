using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Versions.ReleaseVersion;

public interface IReleaseVersion
{
    Task ExecuteAsync(IReleaseVersionOutputPort output, Guid versionId);
}