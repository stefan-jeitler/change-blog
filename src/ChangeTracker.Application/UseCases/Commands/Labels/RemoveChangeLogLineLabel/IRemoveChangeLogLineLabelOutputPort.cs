using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.Labels.RemoveChangeLogLineLabel
{
    public interface IRemoveChangeLogLineLabelOutputPort
    {
        void Removed(Guid lineId);
        void InvalidLabel(string label);
        void Conflict(Conflict conflict);
        void ChangeLogLineDoesNotExist();
    }
}