using System;
using System.Collections.Generic;

namespace ChangeBlog.Application.UseCases.Commands.AddChangeLogLine.Models
{
    public class VersionIdChangeLogLineRequestModel : IChangeLogLineRequestModel
    {
        public VersionIdChangeLogLineRequestModel(Guid userId, Guid versionId,
            string text,
            List<string> labels,
            List<string> issues)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
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
}
