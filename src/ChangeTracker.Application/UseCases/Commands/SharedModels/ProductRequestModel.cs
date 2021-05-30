using System;

namespace ChangeTracker.Application.UseCases.Commands.SharedModels
{
    public class ProductRequestModel
    {
        public ProductRequestModel(Guid accountId, string name, Guid? versioningSchemeId, Guid userId)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (versioningSchemeId.HasValue && versioningSchemeId.Value == Guid.Empty)
                throw new ArgumentException("VersioningSchemeId cannot be empty.");

            VersioningSchemeId = versioningSchemeId;

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;
        }

        public Guid AccountId { get; }
        public string Name { get; }
        public Guid? VersioningSchemeId { get; }
        public Guid UserId { get; }
    }
}