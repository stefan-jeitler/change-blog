using System;

namespace ChangeTracker.Application.UseCases.Commands.SharedModels
{
    public class VersionRequestModel
    {
        public VersionRequestModel(Guid productId,
            Guid userId,
            string name,
            string version, 
            bool releaseImmediately = false)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");

            ProductId = productId;
            
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            Name = name;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            ReleaseImmediately = releaseImmediately;
        }

        public Guid ProductId { get; }
        public Guid UserId { get; }
        public string Name { get; }
        public string Version { get; }
        public bool ReleaseImmediately { get; }
    }
}