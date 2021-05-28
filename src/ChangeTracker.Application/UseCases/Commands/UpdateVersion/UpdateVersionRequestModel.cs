using System;

namespace ChangeTracker.Application.UseCases.Commands.UpdateVersion
{
    public class UpdateVersionRequestModel
    {
        public UpdateVersionRequestModel(Guid versionId, string name, string version)
        {
            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            VersionId = versionId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public Guid VersionId { get; }
        public string Name { get; }
        public string Version { get; }
    }
}