using System;

namespace ChangeBlog.Api.Authorization.RequestBodyIdentifiers
{
    public interface IContainsAccountId
    {
        Guid AccountId { get; }
    }
}