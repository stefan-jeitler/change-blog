using System;

namespace ChangeBlog.Application.UseCases.Commands.UpdateApiKey;

public record UpdateApiKeyRequestModel(Guid ApiKeyId, string Title, DateTime? ExpiresIn);