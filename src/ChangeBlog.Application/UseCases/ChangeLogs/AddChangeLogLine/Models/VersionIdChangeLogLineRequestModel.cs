using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.ChangeLogs.AddChangeLogLine.Models;

public class VersionIdChangeLogLineRequestModel : IChangeLogLineRequestModel
{
    public VersionIdChangeLogLineRequestModel(Guid userId, Guid versionId,
        string text,
        List<string> labels,
        List<string> issues)
    {
        UserId = Guard.Against.NullOrEmpty(userId, nameof(userId));
        VersionId = Guard.Against.NullOrEmpty(versionId, nameof(versionId));

        Text = text ?? throw new ArgumentNullException(nameof(text));
        Labels = labels ?? throw new ArgumentNullException(nameof(labels));
        Issues = issues ?? throw new ArgumentNullException(nameof(issues));
    }

    public Guid VersionId { get; }

    public Guid UserId { get; }
    public string Text { get; }
    public List<string> Labels { get; }
    public List<string> Issues { get; }
}