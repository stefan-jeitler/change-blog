using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.ReleaseVersion
{
    public interface IReleaseVersionOutputPort
    {
        void VersionAlreadyReleased();
        void VersionDeleted();
        void VersionReleased(Guid versionId);
        void Conflict(string reason);
        void VersionDoesNotExist();
        void RelatedProjectClosed(Guid projectId);
    }
}
