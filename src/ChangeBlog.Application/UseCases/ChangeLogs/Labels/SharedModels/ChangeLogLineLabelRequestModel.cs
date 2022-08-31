using System;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.ChangeLogs.Labels.SharedModels;

public class ChangeLogLineLabelRequestModel
{
    public ChangeLogLineLabelRequestModel(Guid changeLogLineId, string label)
    {
        ChangeLogLineId = Guard.Against.NullOrEmpty(changeLogLineId, nameof(changeLogLineId));
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }

    public Guid ChangeLogLineId { get; }
    public string Label { get; }
}