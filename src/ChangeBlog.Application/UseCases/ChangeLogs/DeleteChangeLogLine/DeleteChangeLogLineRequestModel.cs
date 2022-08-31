using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;

public class DeleteChangeLogLineRequestModel
{
    public DeleteChangeLogLineRequestModel(Guid changeLogLineId, ChangeLogLineType changeLogLineType)
    {
        ChangeLogLineId = Guard.Against.NullOrEmpty(changeLogLineId, nameof(changeLogLineId));
        ChangeLogLineType = changeLogLineType;
    }

    public Guid ChangeLogLineId { get; }
    public ChangeLogLineType ChangeLogLineType { get; }
}