using System;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.Application.UseCases.Commands.Labels.AddChangeLogLineLabel
{
    public interface IAddChangeLogLineLabelOutputPort
    {
        void Added(Guid lineId);
        void Conflict(Conflict conflict);
        void ChangeLogLineDoesNotExist();
        void InvalidLabel(string label);
        void MaxLabelsReached(int maxLabels);
    }
}