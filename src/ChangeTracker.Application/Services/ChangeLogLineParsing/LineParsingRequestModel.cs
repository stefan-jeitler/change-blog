using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.Services.ChangeLogLineParsing
{
    public class LineParsingRequestModel
    {
        public LineParsingRequestModel(Guid projectId, Guid? versionId, string text, IList<string> labels,
            IList<string> issues, uint? position = null)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty");

            ProjectId = projectId;

            if (versionId.HasValue && versionId.Value == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
            Position = position;
        }

        public Guid ProjectId { get; }
        public Guid? VersionId { get; }
        public string Text { get; }
        public IList<string> Labels { get; }
        public IList<string> Issues { get; }
        public uint? Position { get; }
    }
}