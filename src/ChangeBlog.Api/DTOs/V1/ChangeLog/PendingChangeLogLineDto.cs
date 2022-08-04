using System;
using ChangeBlog.Application.UseCases.ChangeLogs.GetPendingChangeLogLine;

namespace ChangeBlog.Api.DTOs.V1.ChangeLog;

public class PendingChangeLogLineDto
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public Guid AccountId { get; set; }
    public ChangeLogLineDto ChangeLogLine { get; set; }

    public static PendingChangeLogLineDto FromResponseModel(PendingChangeLogLineResponseModel m)
    {
        return new PendingChangeLogLineDto
        {
            ProductId = m.ProductId,
            ProductName = m.ProductName,
            AccountId = m.AccountId,
            ChangeLogLine = ChangeLogLineDto.FromResponseModel(m.ChangeLogLine)
        };
    }
}