using System;
using System.Collections.Generic;

namespace ChangeTracker.Application.UseCases.Commands.AddCompleteVersion.Models
{
    public class CompleteVersionRequestModel
    {
        public CompleteVersionRequestModel(Guid projectId, string version, List<ChangeLogLineRequestModel> lines,
            bool releaseImmediately = false)
        {
            if (projectId == Guid.Empty)
                throw new ArgumentException("ProjectId cannot be empty.");

            ProjectId = projectId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            ReleaseImmediately = releaseImmediately;
        }

        public Guid ProjectId { get; }
        public string Version { get; }
        public List<ChangeLogLineRequestModel> Lines { get; }
        public bool ReleaseImmediately { get; }
    }
}