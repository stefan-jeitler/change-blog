using System;
using ChangeTracker.Application.Services.ChangeLogLineParsing;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLineOutputPort : IChangeLogLineParsingOutput
    {
        void ProjectDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
    }
}