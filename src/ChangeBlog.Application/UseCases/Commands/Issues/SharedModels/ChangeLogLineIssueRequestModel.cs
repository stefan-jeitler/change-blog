using System;

namespace ChangeBlog.Application.UseCases.Commands.Issues.SharedModels
{
    public class ChangeLogLineIssueRequestModel
    {
        public ChangeLogLineIssueRequestModel(Guid changeLogLineId, string issue)
        {
            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            ChangeLogLineId = changeLogLineId;
            Issue = issue ?? throw new ArgumentNullException(nameof(issue));
        }


        public Guid ChangeLogLineId { get; }
        public string Issue { get; }
    }
}
