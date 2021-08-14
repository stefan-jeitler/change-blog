using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Commands.ReleaseVersion
{
    public interface IReleaseVersion
    {
        Task ExecuteAsync(IReleaseVersionOutputPort output, Guid versionId);
    }
}
