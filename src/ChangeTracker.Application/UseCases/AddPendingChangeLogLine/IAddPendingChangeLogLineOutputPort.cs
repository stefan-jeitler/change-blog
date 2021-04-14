using System;
using ChangeTracker.Application.Services.ChangeLog;

namespace ChangeTracker.Application.UseCases.AddPendingChangeLogLine
{
    public interface IAddPendingChangeLogLineOutputPort : IChangeLogLineParsingOutput
    {
        void ProjectDoesNotExist();
        void Created(Guid changeLogLineId);
        void Conflict(string reason);
    }
}