using System;

namespace ChangeTracker.Api.Authorization.RequestBodyIdentifiers
{
    public interface IContainsAccountId
    {
        Guid AccountId { get; }
    }
}
