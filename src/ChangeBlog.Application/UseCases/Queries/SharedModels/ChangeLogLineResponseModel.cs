using System;
using System.Collections.Generic;

namespace ChangeBlog.Application.UseCases.Queries.SharedModels;

public class ChangeLogLineResponseModel
{
    public ChangeLogLineResponseModel(Guid id, string text, List<string> labels, List<string> issues,
        DateTimeOffset createdAt)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.");

        Id = id;

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