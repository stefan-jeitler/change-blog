using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Commands.AddPendingChangeLogLine
{
    public class PendingChangeLogLineRequestModel
    {
        public PendingChangeLogLineRequestModel(Guid userId, Guid productId, string text, List<string> labels,
            List<string> issues)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            Text = text ?? throw new ArgumentNullException(nameof(text));
            Labels = labels ?? throw new ArgumentNullException(nameof(labels));
            Issues = issues ?? throw new ArgumentNullException(nameof(issues));
        }

        public Guid UserId { get; }
        public Guid ProductId { get; }
        public string Text { get; }
        public List<string> Labels { get; }
        public List<string> Issues { get; }
    }
}