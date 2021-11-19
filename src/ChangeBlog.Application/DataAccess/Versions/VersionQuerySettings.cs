using System;

namespace ChangeBlog.Application.DataAccess.Versions;

public class VersionQuerySettings
{
    public VersionQuerySettings(Guid productId, Guid? lastVersionId, string searchTerm, ushort limit,
        bool includeDeleted)
    {
        ProductId = productId;
        LastVersionId = lastVersionId;
        SearchTerm = searchTerm;
        Limit = limit;
        IncludeDeleted = includeDeleted;
    }

    public Guid ProductId { get; }

    public Guid? LastVersionId { get; }

    public string SearchTerm { get; }

    public ushort Limit { get; }

    public bool IncludeDeleted { get; }
}