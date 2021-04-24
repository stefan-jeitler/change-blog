using System;

namespace ChangeTracker.Application.UseCases.Command.AddProject
{
    public class ProjectRequestModel
    {
        public ProjectRequestModel(Guid accountId, string name, Guid? versioningSchemeId)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (versioningSchemeId.HasValue && versioningSchemeId.Value == Guid.Empty)
                throw new ArgumentException("VersioningSchemeId cannot be empty.");

            VersioningSchemeId = versioningSchemeId;
        }

        public Guid AccountId { get; }
        public string Name { get; }
        public Guid? VersioningSchemeId { get; }
    }
}