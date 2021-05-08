using System;

namespace ChangeTracker.Application.UseCases.Commands.Labels.RemoveChangeLogLineLabel
{
    public interface IRemoveChangeLogLineLabelOutputPort
    {
        void Removed(Guid lineId);
        void InvalidLabel(string label);
        void Conflict(string reason);
        void ChangeLogLineDoesNotExist();
    }
}