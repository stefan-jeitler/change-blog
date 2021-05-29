﻿using System;
using ChangeTracker.Application.UseCases.Queries.GetProducts;

namespace ChangeTracker.Api.DTOs.V1.Product
{
    public class ProductDto
    {
        public ProductDto(Guid id, Guid accountId, string accountName, string name, Guid versioningSchemeId,
            string versioningScheme, string createdByUser, DateTime createdAt, bool isClosed)
        {
            Id = id;
            AccountId = accountId;
            AccountName = accountName;
            Name = name;
            VersioningSchemeId = versioningSchemeId;
            VersioningScheme = versioningScheme;
            CreatedByUser = createdByUser;
            CreatedAt = createdAt;
            IsClosed = isClosed;
        }

        public Guid Id { get; }
        public Guid AccountId { get; }
        public string AccountName { get; }
        public string Name { get; }
        public Guid VersioningSchemeId { get; }
        public string VersioningScheme { get; }
        public string CreatedByUser { get; }
        public DateTime CreatedAt { get; }
        public bool IsClosed { get; }

        public static ProductDto FromResponseModel(ProductResponseModel m) =>
            new(
                m.Id,
                m.AccountId,
                m.AccountName,
                m.Name,
                m.VersioningSchemeId,
                m.VersioningScheme,
                m.CreatedByUser,
                m.CreatedAt,
                m.ClosedAt.HasValue);
    }
}