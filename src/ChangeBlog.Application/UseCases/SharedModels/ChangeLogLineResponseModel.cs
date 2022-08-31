using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;

namespace ChangeBlog.Application.UseCases.SharedModels;

public class ChangeLogLineResponseModel
{
    public ChangeLogLineResponseModel(Guid id,
        string text,
        List<string> labels,
        List<string> issues,
        DateTimeOffset createdAt)
    {
        Id = Guard.Against.NullOrEmpty(id, nameof(id));
        Text = text ?? throw new ArgumentNullException(nameof(text));
        Labels = labels ?? throw new ArgumentNullException(nameof(labels));
        Issues = issues ?? throw new ArgumentNullException(nameof(issues));

        if (createdAt == DateTimeOffset.MinValue || createdAt == DateTimeOffset.MaxValue)
            throw new ArgumentException("Invalid creation date.");

        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public string Text { get; }
    public List<string> Labels { get; }
    public List<string> Issues { get; }
    public DateTimeOffset CreatedAt { get; }
}