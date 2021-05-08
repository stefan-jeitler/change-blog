using System;

namespace ChangeTracker.Application.UseCases.Commands.Labels.SharedModels
{
    public class ChangeLogLineLabelRequestModel
    {
        public ChangeLogLineLabelRequestModel(Guid changeLogLineId, string label)
        {
            if (changeLogLineId == Guid.Empty)
                throw new ArgumentException("ChangeLogLineId cannot be empty.");

            ChangeLogLineId = changeLogLineId;
            Label = label ?? throw new ArgumentNullException(nameof(label));
        }

        public Guid ChangeLogLineId { get; }
        public string Label { get; }
    }
}