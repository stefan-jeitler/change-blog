using System;
using System.ComponentModel.DataAnnotations;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;

namespace ChangeTracker.Api.DTOs.V1.Version
{
    public class AddVersionDto : IContainsProductId
    {
        [Required] public string Version { get; set; }

        [Required] public Guid ProductId { get; set; }
    }
}