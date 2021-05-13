using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodIdentifiers;

namespace ChangeTracker.Api.DTOs.v1.Project
{
    public class AddProjectDto : IContainsAccountId
    {
        [Required]
        public Guid AccountId { get; set; }
        [Required]
        public string Name { get; set; }
        public Guid? VersioningSchemeId { get; set; }
    }
}
