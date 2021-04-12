using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AssignPendingLineToVersion
{
    public interface IAssignPendingLineOutputPort
    {
        void InvalidVersionFormat(string version);
        void VersionDoesNotExist();
        void MaxChangeLogLinesReached(int maxChangeLogLines);
        void ChangeLogLineDoesNotExist();
        void Conflict(string reason);
        void Assigned(Guid versionId, Guid changeLogLineId);
    }
}
