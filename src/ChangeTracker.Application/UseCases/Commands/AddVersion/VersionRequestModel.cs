using System;

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
{
    public class VersionRequestModel
    {
        public VersionRequestModel(Guid productId, string version)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            Version = version ?? throw new ArgumentNullException(nameof(version));
        }

        public Guid ProductId { get; }
        public string Version { get; }

        public void Deconstruct(out Guid productId, out string version)
        {
            (productId, version) = (ProductId, Version);
        }
    }
}