using System;

namespace ChangeBlog.Application.UseCases.Users.UpdateApiKey;

public record UpdateApiKeyRequestModel(Guid ApiKeyId, string Name, DateTime? ExpiresIn);