using System;
using System.Collections.Generic;
using Ardalis.GuardClauses;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogs;

public class PendingChangeLogsResponseModel
{
    public PendingChangeLogsResponseModel(Guid productId, string productName, Guid accountId,
        List<ChangeLogLineResponseModel> changeLogs)
    {
        ProductId = Guard.Against.NullOrEmpty(productId, nameof(productId));
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        AccountId = Guard.Against.NullOrEmpty(accountId, nameof(accountId));
        ChangeLogs = changeLogs ?? throw new ArgumentNullException(nameof(changeLogs));
    }

    public Guid ProductId { get; }
    public string ProductName { get; }
    public Guid AccountId { get; }
    public List<ChangeLogLineResponseModel> ChangeLogs { get; }
}