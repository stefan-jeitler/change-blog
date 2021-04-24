using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.AddChangeLogLine.Models
{
    public class VersionIdChangeLogLineRequestModel : IChangeLogLine
    {
        public VersionIdChangeLogLineRequestModel(Guid versionId,
            string text,
            List<string> labels,
            List<string> issues)
        {
            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }
        
        public Guid VersionId { get; }
        public string Text { get; }
        public List<string> Labels { get; }
        public List<string> Issues { get; }
    }
}
