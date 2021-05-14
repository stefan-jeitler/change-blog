using System;
using System.ComponentModel.DataAnnotations;
using ChangeTracker.Api.Authorization.RequestBodyIdentifiers;

namespace ChangeTracker.Api.DTOs.v1.Project
{
    public class AddProjectDto : IContainsAccountId
    {
        [Required] public string Name { get; set; }

        public Guid? VersioningSchemeId { get; set; }

        [Required] public Guid AccountId { get; set; }
    }
}