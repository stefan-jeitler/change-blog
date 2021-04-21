using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AssignAllPendingLinesToVersion
{
    public interface IAssignAllPendingLinesToVersionOutputPort
    {
        void Assigned(Guid versionId);
        void InvalidVersionFormat(string version);
        void VersionDoesNotExist();
        void TooManyLinesToAdd(uint remainingLinesToAdd);
        void Conflict(string reason);
    }
}
