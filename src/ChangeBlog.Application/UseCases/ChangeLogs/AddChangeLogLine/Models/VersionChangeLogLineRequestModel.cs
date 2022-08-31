using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine.Models;

public class VersionChangeLogLineRequestModel : IChangeLogLineRequestModel
{
    public VersionChangeLogLineRequestModel(Guid userId,
        Guid productId,
        string version,
        string text,
        List<string> labels,
        List<string> issues)
    {
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
        ProductId = Guard.Against.NullOrEmpty(productId, nameof(productId));
        
        Version = version ?? throw new ArgumentNullException(nameof(version));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Labels = labels ?? throw new ArgumentNullException(nameof(labels));
        Issues = issues ?? throw new ArgumentNullException(nameof(issues));
    }

    public Guid ProductId { get; }
    public string Version { get; }

    public Guid UserId { get; }
    public string Text { get; }
    public List<string> Labels { get; }
    public List<string> Issues { get; }
}