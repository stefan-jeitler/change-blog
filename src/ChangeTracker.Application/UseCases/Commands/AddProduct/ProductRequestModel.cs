using System;

namespace ChangeTracker.Application.UseCases.Commands.AddProduct
{
    public class ProductRequestModel
    {
        public ProductRequestModel(Guid accountId, string name, Guid? versioningSchemeId, string languageCode, Guid userId)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (versioningSchemeId.HasValue && versioningSchemeId.Value == Guid.Empty)
                throw new ArgumentException("VersioningSchemeId cannot be empty.");

            VersioningSchemeId = versioningSchemeId;

            LanguageCode = languageCode?.ToLower() ?? throw new ArgumentNullException(nameof(languageCode));

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;
        }

        public Guid AccountId { get; }
        public string Name { get; }
        public Guid? VersioningSchemeId { get; }
        public string LanguageCode { get; }
        public Guid UserId { get; }
    }
}