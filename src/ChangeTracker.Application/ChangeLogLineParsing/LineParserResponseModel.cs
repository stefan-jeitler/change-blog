using System;
using System.Collections.Generic;
using ChangeTracker.Domain.ChangeLog;

namespace ChangeTracker.Application.ChangeLogLineParsing
{
    public class LineParserResponseModel
    {
        public LineParserResponseModel(ChangeLogText text, List<Label> labels, List<Issue> issues)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public ChangeLogText Text { get; }
        public List<Label> Labels { get; }
        public List<Issue> Issues { get; }
    }
}