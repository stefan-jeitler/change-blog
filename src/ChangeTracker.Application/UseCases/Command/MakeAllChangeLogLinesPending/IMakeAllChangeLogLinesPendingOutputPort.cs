using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Command.MakeAllChangeLogLinesPending
{
    public interface IMakeAllChangeLogLinesPendingOutputPort
    {
        void VersionDoesNotExist();
        void VersionAlreadyReleased();
        void VersionDeleted();
        void TooManyPendingLines(int maxChangeLogLines);
        void LineWithSameTextAlreadyExists(List<string> text);
        void Conflict(string reason);
        void MadePending(int count);
        void InvalidVersionFormat(string version);
    }
}
