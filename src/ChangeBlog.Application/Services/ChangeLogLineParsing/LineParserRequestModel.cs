using System;
using System.Collections.Generic;

namespace ChangeBlog.Application.Services.ChangeLogLineParsing
{
    public class LineParserRequestModel
    {
        public LineParserRequestModel(string text, IList<string> labels, IList<string> issues)
        {
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public string Text { get; }
        public IList<string> Labels { get; }
        public IList<string> Issues { get; }
    }
}
