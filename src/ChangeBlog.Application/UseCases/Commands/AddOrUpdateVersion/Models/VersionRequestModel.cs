using System;
using System.Collections.Generic;

namespace ChangeBlog.Application.UseCases.Commands.AddOrUpdateVersion.Models
{
    public class VersionRequestModel
    {
        public VersionRequestModel(Guid userId, Guid productId, string version, string name,
            List<ChangeLogLineRequestModel> lines,
            bool releaseImmediately = false)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Name = name;
            Lines = lines ?? throw new ArgumentNullException(nameof(lines));
            ReleaseImmediately = releaseImmediately;
        }

        public Guid UserId { get; }
        public Guid ProductId { get; }
        public string Version { get; }
        public string Name { get; }
        public List<ChangeLogLineRequestModel> Lines { get; }
        public bool ReleaseImmediately { get; }
    }
}
