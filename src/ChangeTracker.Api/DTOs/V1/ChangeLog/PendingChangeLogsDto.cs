﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogs;

namespace ChangeTracker.Api.DTOs.V1.ChangeLog
{
    public class PendingChangeLogsDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid AccountId { get; set; }
        public List<ChangeLogLineDto> ChangeLogLines { get; set; }

        public static PendingChangeLogsDto FromResponseModel(PendingChangeLogsResponseModel m) =>
            new()
            {
                ProductId = m.ProductId,
                ProductName = m.ProductName,
                AccountId = m.AccountId,
                ChangeLogLines = m.ChangeLogs.Select(ChangeLogLineDto.FromResponseModel).ToList(),
            };
    }
}