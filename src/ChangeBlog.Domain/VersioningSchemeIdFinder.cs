using System;

namespace ChangeBlog.Domain;

public class VersioningSchemeIdFinder
{
    private readonly Account _account;

    public VersioningSchemeIdFinder(Account account)
    {
        _account = account;
    }

    public Guid FindSchemeIdForProduct(Guid? customVersioningSchemeId)
    {
        if (customVersioningSchemeId.HasValue)
        {
            return customVersioningSchemeId.Value;
        }

        return _account.DefaultVersioningSchemeId ?? Defaults.VersioningSchemeId;
    }
}