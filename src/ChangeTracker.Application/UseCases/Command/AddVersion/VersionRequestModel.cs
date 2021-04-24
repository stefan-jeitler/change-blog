using System;

namespace ChangeTracker.Application.UseCases.Command.AddVersion
{
    public class VersionRequestModel
    {
        public VersionRequestModel(Guid projectId, string version)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public Guid ProjectId { get; }
        public string Version { get; }

        public void Deconstruct(out Guid projectId, out string version)
        {
            (projectId, version) = (ProjectId, Version);
        }
    }
}