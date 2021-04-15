using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine
{
    public class ChangeLogLineRequestModel
    {
        public ChangeLogLineRequestModel(Guid projectId,
            string version,
            string text,
            List<string> labels,
            List<string> issues)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public Guid ProjectId { get; }
        public string Version { get; }
        public string Text { get; }
        public List<string> Labels { get; }
        public List<string> Issues { get; }
    }
}