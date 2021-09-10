using System;
using System.Collections.Immutable;
using ChangeBlog.Domain.ChangeLog;

namespace ChangeBlog.Application.ChangeLogLineParser
{
    public class LineParserResponseModel
    {
        public LineParserResponseModel(ChangeLogText text, IImmutableSet<Label> labels, IImmutableSet<Issue> issues)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public ChangeLogText Text { get; }
        public IImmutableSet<Label> Labels { get; }
        public IImmutableSet<Issue> Issues { get; }
    }
}
