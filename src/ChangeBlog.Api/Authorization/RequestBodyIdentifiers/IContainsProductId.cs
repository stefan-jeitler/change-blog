using System;

namespace ChangeBlog.Api.Authorization.RequestBodyIdentifiers;

public interface IContainsProductId
{
    Guid ProductId { get; }
}