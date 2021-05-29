using System;

namespace ChangeTracker.Application.UseCases.Commands.SharedModels
{
    public class VersionRequestModel
    {
        public VersionRequestModel(Guid userId,
            Guid productId,
            string version,
            string name, 
            bool releaseImmediately = false)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;
            
            if (productId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            ProductId = productId;

            Name = name;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            ReleaseImmediately = releaseImmediately;
        }

        public Guid UserId { get; }
        public Guid ProductId { get; }
        public string Name { get; }
        public string Version { get; }
        public bool ReleaseImmediately { get; }
    }
}