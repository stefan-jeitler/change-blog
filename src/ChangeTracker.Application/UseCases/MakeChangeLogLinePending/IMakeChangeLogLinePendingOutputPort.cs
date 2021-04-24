using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.MakeChangeLogLinePending
{
    public interface IMakeChangeLogLinePendingOutputPort
    {
        void WasMadePending(Guid lineId);
        void ChangeLogLineDoesNotExist();
        void ChangeLogLineIsAlreadyPending();
        void VersionAlreadyReleased();
        void VersionDeleted();
        void TooManyPendingLines(int maxChangeLogLines);
        void Conflict(string reason);
        void LineWithSameTextAlreadyExists(string text);
    }
}
