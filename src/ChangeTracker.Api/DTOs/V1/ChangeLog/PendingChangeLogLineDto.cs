﻿using System;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogLine;

namespace ChangeTracker.Api.DTOs.V1.ChangeLog
{
    public class PendingChangeLogLineDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid AccountId { get; set; }
        public ChangeLogLineDto ChangeLogLine { get; set; }

        public static PendingChangeLogLineDto FromResponseModel(PendingChangeLogLineResponseModel m) =>
            new()
            {
                ProductId = m.ProductId,
                ProductName = m.ProductName,
                AccountId = m.AccountId,
                ChangeLogLine = ChangeLogLineDto.FromResponseModel(m.ChangeLogs)
            };
    }
}