using System;

namespace ChangeTracker.Api.Authorization.RequestBodyIdentifiers
{
    public interface IContainsProductId
    {
        Guid ProductId { get; }
    }
}