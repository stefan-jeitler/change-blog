using System;
using ChangeTracker.Application.UseCases.Queries.GetProducts;

namespace ChangeTracker.Api.DTOs.v1.Product
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public Guid VersioningSchemeId { get; set; }
        public string VersioningScheme { get; set; }
        public string CreatedByUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }

        public static ProductDto FromResponseModel(ProductResponseModel m)
        {
            return new()
            {
                Id = m.Id,
                AccountId = m.AccountId,
                Name = m.Name,
                VersioningSchemeId = m.VersioningSchemeId,
                VersioningScheme = m.VersioningScheme,
                CreatedByUser = m.CreatedByUser,
                CreatedAt = m.CreatedAt,
                IsClosed = m.ClosedAt.HasValue
            };
        }
    }
}