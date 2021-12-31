using System;
using ChangeBlog.Application.UseCases.Queries.SharedModels;

namespace ChangeBlog.Application.UseCases.Queries.GetPendingChangeLogLine;

public class PendingChangeLogLineResponseModel
{
    public PendingChangeLogLineResponseModel(Guid productId, string productName, Guid accountId,
        ChangeLogLineResponseModel changeLogLine)
    {
        if (productId == Guid.Empty)
        {
            throw new ArgumentException("ProductId cannot be empty.");
        }

        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));

        if (accountId == Guid.Empty)
        {
            throw new ArgumentException("AccountId cannot be empty.");
        }

        AccountId = accountId;

        ChangeLogLine = changeLogLine ?? throw new ArgumentNullException(nameof(changeLogLine));
    }

    public Guid ProductId { get; }
    public string ProductName { get; }
    public Guid AccountId { get; }
    public ChangeLogLineResponseModel ChangeLogLine { get; }
}