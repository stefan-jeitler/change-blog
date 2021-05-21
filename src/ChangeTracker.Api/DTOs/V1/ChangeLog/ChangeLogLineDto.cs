using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Api.DTOs.V1.ChangeLog
{
    public class ChangeLogLineDto
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public List<string> Labels { get; set; }
        public List<string> Issues { get; set; }
        public DateTime CreatedAt { get; set; }

        public static ChangeLogLineDto FromResponseModel(ChangeLogLineResponseModel model)
        {
            return new()
            {
                Id = model.Id,
                Text = model.Text,
                Labels = model.Labels,
                Issues = model.Issues,
                CreatedAt = model.CreatedAt
            };
        }
    }
}