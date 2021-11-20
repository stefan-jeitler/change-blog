using System;

namespace ChangeBlog.Application.Models;

public record ExternalIdentity(Guid Id,
    Guid UserId,
    string ExternalUserId,
    string IdentityProvider,
    DateTime CreatedAt);