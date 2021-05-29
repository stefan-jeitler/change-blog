using System;
using System.ComponentModel.DataAnnotations;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;

namespace ChangeTracker.Api.DTOs.V1.Version
{
    public class AddOrUpdateVersionDto : IContainsProductId
    {
        [Required] public string Version { get; set; }

        public string Name { get; set; }
        public bool ReleaseImmediately { get; set; }

        [Required] public Guid ProductId { set; get; }
    }
}