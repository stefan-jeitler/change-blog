using System;

namespace ChangeBlog.Api.Shared.Authorization.RequestBodyIdentifiers;

public interface IContainsProductId
{
    Guid ProductId { get; }
}