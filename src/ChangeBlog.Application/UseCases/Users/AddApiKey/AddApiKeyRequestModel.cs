using System;

namespace ChangeBlog.Application.UseCases.Users.AddApiKey;

public record AddApiKeyRequestModel(Guid UserId, string Name, DateTime ExpiresAt);