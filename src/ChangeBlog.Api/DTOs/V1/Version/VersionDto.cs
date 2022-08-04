using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Api.DTOs.V1.ChangeLog;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Api.DTOs.V1.Version;

public class VersionDto
{
    public Guid Id { get; set; }
    public string Version { get; set; }
    public string Name { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public Guid AccountId { get; set; }
    public List<ChangeLogLineDto> ChangeLogLines { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ReleasedAt { get; set; }
    public DateTimeOffset? DeletedAt { get; set; }

    public static VersionDto FromResponseModel(VersionResponseModel model)
    {
        return new VersionDto
        {
            Id = model.VersionId,
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