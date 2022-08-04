using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.ChangeLogs.DeleteChangeLogLine;

namespace ChangeBlog.Application.UseCases.ChangeLogs.UpdateChangeLogLine;

public class UpdateChangeLogLineRequestModel
{
    public UpdateChangeLogLineRequestModel(Guid changeLogLineId, ChangeLogLineType changeLogLineType, string text,
        IList<string> labels, IList<string> issues)
    {
        if (changeLogLineId == Guid.Empty)
        {
            throw new ArgumentException("ChangeLogLineId cannot be empty.");
        }

        ChangeLogLineId = changeLogLineId;
        ChangeLogLineType = changeLogLineType;
        Text = text;
        Labels = labels;
        Issues = issues;
    }

    public Guid ChangeLogLineId { get; }
    public ChangeLogLineType ChangeLogLineType { get; }
    public string Text { get; }
    public IList<string> Labels { get; }
    public IList<string> Issues { get; }
}