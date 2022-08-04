using System;
using System.Collections.Generic;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog;

public class ChangeLogLineDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public List<string> Labels { get; set; }
    public List<string> Issues { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public static ChangeLogLineDto FromResponseModel(ChangeLogLineResponseModel model)
    {
        return new ChangeLogLineDto
        {
            Id = model.Id,
            Text = model.Text,
            Labels = model.Labels,
            Issues = model.Issues,
            CreatedAt = model.CreatedAt
        };
    }
}