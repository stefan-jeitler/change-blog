using System;
using Ardalis.GuardClauses;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogLine;

public class PendingChangeLogLineResponseModel
{
    public PendingChangeLogLineResponseModel(Guid productId, string productName, Guid accountId,
        ChangeLogLineResponseModel changeLogLine)
    {
        ProductId = Guard.Against.NullOrEmpty(productId, nameof(productId));
        ProductName = Guard.Against.NullOrEmpty(productName, nameof(productName));
        AccountId = Guard.Against.NullOrEmpty(accountId, nameof(accountId));

        ChangeLogLine = changeLogLine ?? throw new ArgumentNullException(nameof(changeLogLine));
    }

    public Guid ProductId { get; }
    public string ProductName { get; }
    public Guid AccountId { get; }
    public ChangeLogLineResponseModel ChangeLogLine { get; }
}