using System;

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
{
    public class VersionRequestModel
    {
        public VersionRequestModel(Guid userId, Guid productId, string version, string name)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            Name = name;
        }

        public Guid UserId { get; }
        public Guid ProductId { get; }
        public string Version { get; }
        public string Name { get; }
    }
}