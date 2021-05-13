using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.ReleaseVersion
{
    public interface IReleaseVersion
    {
        Task ExecuteAsync(IReleaseVersionOutputPort output, Guid versionId);

    }
}
