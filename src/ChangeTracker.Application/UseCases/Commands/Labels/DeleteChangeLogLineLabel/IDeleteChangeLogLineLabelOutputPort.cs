using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel
{
    public interface IDeleteChangeLogLineLabelOutputPort
    {
        void Deleted(Guid changeLogLineId);
        void InvalidLabel(string label);
        void Conflict(Conflict conflict);
        void ChangeLogLineDoesNotExist();
    }
}