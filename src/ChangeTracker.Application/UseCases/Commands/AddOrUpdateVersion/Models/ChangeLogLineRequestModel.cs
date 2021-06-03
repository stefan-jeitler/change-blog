using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion.Models
{
    public class ChangeLogLineRequestModel
    {
        public ChangeLogLineRequestModel(string text, List<string> labels, List<string> issues)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public string Text { get; }
        public List<string> Labels { get; }
        public List<string> Issues { get; }
    }
}