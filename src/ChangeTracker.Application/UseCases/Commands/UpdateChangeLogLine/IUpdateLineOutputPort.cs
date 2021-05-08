using System;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.Commands.UpdateChangeLogLine
{
    public interface IUpdateLineOutputPort : ILineParserOutput
    {
        void Updated(Guid lineId);
        void ChangeLogLineDoesNotExist();
        void Conflict(string reason);
        void LineWithSameTextAlreadyExists(string text);
    }
}