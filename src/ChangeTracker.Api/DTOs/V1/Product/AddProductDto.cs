using System;
using System.ComponentModel.DataAnnotations;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;

namespace ChangeTracker.Api.DTOs.V1.Product
{
    public class AddProductDto : IContainsAccountId
    {
        [Required] public string Name { get; set; }

        public Guid? VersioningSchemeId { get; set; }

        [Required] public Guid AccountId { get; set; }
    }
}