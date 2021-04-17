using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.UpdateChangeLogLine
{
    public interface IUpdateLineOutputPort : ILineParserOutput
    {
        void Updated(Guid lineId);
        void ChangeLogLineDoesNotExist();
        void NotModified();
        void Conflict(string reason);
        void AppropriateVersionAlreadyReleased();
        void AppropriateVersionDeleted();
    }
}
