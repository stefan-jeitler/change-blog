using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddPendingChangeLogLine;

public class PendingChangeLogLineRequestModel
{
    public PendingChangeLogLineRequestModel(Guid userId, Guid productId, string text, List<string> labels,
        List<string> issues)
    {
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
        ProductId = Guard.Against.NullOrEmpty(productId, nameof(productId));

        Text = text ?? throw new ArgumentNullException(nameof(text));
        Labels = labels ?? throw new ArgumentNullException(nameof(labels));
        Issues = issues ?? throw new ArgumentNullException(nameof(issues));
    }

    public Guid UserId { get; }
    public Guid ProductId { get; }
    public string Text { get; }
    public List<string> Labels { get; }
    public List<string> Issues { get; }
}