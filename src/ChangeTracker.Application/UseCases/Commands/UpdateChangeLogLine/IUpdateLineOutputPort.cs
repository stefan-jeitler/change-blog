using System;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.UpdateChangeLogLine
{
    public interface IUpdateLineOutputPort : ILineParserOutput
    {
        void Updated(Guid lineId);
        void ChangeLogLineDoesNotExist();
        void Conflict(Conflict conflict);
        void LineWithSameTextAlreadyExists(string text);
    }
}