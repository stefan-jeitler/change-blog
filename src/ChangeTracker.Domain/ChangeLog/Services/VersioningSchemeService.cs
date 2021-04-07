using System;
using ChangeTracker.Domain.Version;

namespace ChangeTracker.Domain.ChangeLog.Services
{
    public class VersioningSchemeService
    {
        private readonly Account _account;

        public VersioningSchemeService(Account account)
        {
            _account = account;
        }

        public Guid FindSchemeIdForProject(Guid? customVersioningSchemeId)
        {
            if (customVersioningSchemeId.HasValue)
                return customVersioningSchemeId.Value;

            return _account.DefaultVersioningSchemeId ?? Defaults.VersioningScheme.Id;
        }
    }
}