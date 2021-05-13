using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.DeleteVersion
{
    public interface IDeleteVersionOutputPort
    {
        void VersionDoesNotExist();
        void ProjectClosed(Guid projectId);
        void VersionAlreadyDeleted();
        void VersionAlreadyReleased();
        void VersionDeleted(Guid versionId);
        void Conflict(string reason);
    }
}
