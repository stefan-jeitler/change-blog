using System;
using ChangeBlog.Application.Boundaries.DataAccess;

namespace ChangeBlog.Application.UseCases.Commands.Labels.DeleteChangeLogLineLabel;

public interface IDeleteChangeLogLineLabelOutputPort
{
    void Deleted(Guid changeLogLineId);
    void InvalidLabel(string label);
    void Conflict(Conflict conflict);
    void ChangeLogLineDoesNotExist();
}