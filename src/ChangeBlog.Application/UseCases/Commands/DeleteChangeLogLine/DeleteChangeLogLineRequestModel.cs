using System;

namespace ChangeBlog.Application.UseCases.Commands.DeleteChangeLogLine;

public class DeleteChangeLogLineRequestModel
{
    public DeleteChangeLogLineRequestModel(Guid changeLogLineId, ChangeLogLineType changeLogLineType)
    {
        if (changeLogLineId == Guid.Empty)
        {
            throw new ArgumentException("ChangeLogLineId cannot be empty.");
        }

        ChangeLogLineId = changeLogLineId;
        ChangeLogLineType = changeLogLineType;
    }

    public Guid ChangeLogLineId { get; }
    public ChangeLogLineType ChangeLogLineType { get; }
}