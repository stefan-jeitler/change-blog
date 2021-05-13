using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Api.Authorization.RequestBodIdentifiers;

namespace ChangeTracker.Api.DTOs.v1.Project
{
    public class AddProjectDto : IContainsAccountId
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public Guid? VersioningSchemeId { get; set; }
    }
}
