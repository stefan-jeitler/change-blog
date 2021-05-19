using System;

namespace ChangeTracker.Domain
{
    public class VersioningSchemeService
    {
        private readonly Account _account;

        public VersioningSchemeService(Account account)
        {
            _account = account;
        }

        public Guid FindSchemeIdForProduct(Guid? customVersioningSchemeId)
        {
            if (customVersioningSchemeId.HasValue)
                return customVersioningSchemeId.Value;

            return _account.DefaultVersioningSchemeId ?? Defaults.VersioningSchemeId;
        }
    }
}