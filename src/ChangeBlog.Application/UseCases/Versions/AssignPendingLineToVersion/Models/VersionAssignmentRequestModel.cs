using System;

namespace ChangeBlog.Application.UseCases.Versions.AssignPendingLineToVersion.Models;

public class VersionAssignmentRequestModel
{
    public VersionAssignmentRequestModel(Guid productId, string version, Guid changeLogLineId)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty.");
        }

        ProductId = productId;
        Version = version ?? throw new ArgumentNullException(nameof(version));

        if (changeLogLineId == Guid.Empty)
        {
            throw new ArgumentException("LineId cannot be empty.");
        }

        ChangeLogLineId = changeLogLineId;
    }

    public Guid ProductId { get; }
    public string Version { get; }
    public Guid ChangeLogLineId { get; }
}