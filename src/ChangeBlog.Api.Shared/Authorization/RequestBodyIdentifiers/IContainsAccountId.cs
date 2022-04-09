using System;

namespace ChangeBlog.Api.Shared.Authorization.RequestBodyIdentifiers;

public interface IContainsAccountId
{
    Guid AccountId { get; }
}