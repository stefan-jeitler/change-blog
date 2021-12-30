using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.Labels.AddChangeLogLineLabel;

public interface IAddChangeLogLineLabelOutputPort
{
    void Added(Guid changeLogLineId);
    void Conflict(Conflict conflict);
    void ChangeLogLineDoesNotExist();
    void InvalidLabel(string label);
    void MaxLabelsReached(int maxLabels);
}