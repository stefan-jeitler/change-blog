using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetProjects
{
    public class ProjectResponseModel
    {
        public ProjectResponseModel(Guid id, Guid accountId, string name, Guid versioningSchemeId,
            string versioningScheme, string createdByUser, DateTime createdAt)
        {
            Id = id;
            AccountId = accountId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            VersioningSchemeId = versioningSchemeId;
            VersioningScheme = versioningScheme ?? throw new ArgumentNullException(nameof(versioningScheme));
            CreatedByUser = createdByUser ?? throw new ArgumentNullException(nameof(createdByUser));
            CreatedAt = createdAt;
        }

        public Guid Id { get; }
        public Guid AccountId { get; }
        public string Name { get; }
        public Guid VersioningSchemeId { get; }
        public string VersioningScheme { get; }
        public string CreatedByUser { get; }
        public DateTime CreatedAt { get; }
    }
}
