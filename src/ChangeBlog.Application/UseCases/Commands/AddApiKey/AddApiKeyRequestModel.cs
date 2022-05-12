using System;

namespace ChangeBlog.Application.UseCases.Commands.AddApiKey;

public record AddApiKeyRequestModel(Guid UserId, string Title, DateTime ExpiresAt);