using System;
using System.Collections.Generic;
using System.Linq;
using ChangeTracker.Api.DTOs.V1.ChangeLog;
using ChangeTracker.Application.UseCases.Queries.GetCompleteVersions;

namespace ChangeTracker.Api.DTOs.V1.Version
{
    public class CompleteVersionDto
    {
        public Guid VersionId { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public Guid AccountId { get; set; }
        public List<ChangeLogLineDto> ChangeLogLines { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReleasedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public static CompleteVersionDto FromResponseModel(CompleteVersionResponseModel model) =>
            new()
            {
                VersionId = model.VersionId,
                Version = model.Version,
                Name = model.Name,
                ProductId = model.ProductId,
                ProductName = model.ProductName,
                AccountId = model.AccountId,
                ChangeLogLines = model.ChangeLogs.Select(ChangeLogLineDto.FromResponseModel).ToList(),
                CreatedAt = model.CreatedAt,
                ReleasedAt = model.ReleasedAt,
                DeletedAt = model.DeletedAt
            };
    }
}