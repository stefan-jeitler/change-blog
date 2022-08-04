using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.Models;
using ChangeBlog.Application.UseCases.Users.AddApiKey;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Users.UpdateApiKey;

public class UpdateApiKeyInteractor : IUpdateApiKey
{
    private readonly IApiKeysDao _apiKeysDao;
    private readonly IUserDao _userDao;

    public UpdateApiKeyInteractor(IApiKeysDao apiKeysDao, IUserDao userDao)
    {
        _apiKeysDao = apiKeysDao ?? throw new ArgumentNullException(nameof(apiKeysDao));
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
    }

    public async Task ExecuteAsync(IUpdateApiKeyOutputPort output, UpdateApiKeyRequestModel requestModel)
    {
        var existingApiKey = await _apiKeysDao.GetApiKeyAsync(requestModel.ApiKeyId);

        if (existingApiKey.HasNoValue)
        {
            output.ApiKeyNotFound(requestModel.ApiKeyId);
            return;
        }

        var updateTitle = requestModel.Title is not null;
        OptionalName title = null;
        if (updateTitle && !OptionalName.TryParse(requestModel.Title, out title))
        {
            output.InvalidTitle(requestModel.Title);
            return;
        }

        var expiresAt = await GetExpirationDateAsync(output, existingApiKey.GetValueOrDefault(), requestModel.ExpiresIn);
        if (expiresAt.HasNoValue)
            return;

        var apiKey = existingApiKey.GetValueOrThrow();

        var updatedApiKey = new ApiKey(apiKey.UserId,
            apiKey.ApiKeyId,
            updateTitle ? title : apiKey.Title,
            apiKey.Key,
            expiresAt.GetValueOrDefault());

        await UpdateApiKeyAsync(output, updatedApiKey);
    }

    private async Task UpdateApiKeyAsync(IUpdateApiKeyOutputPort output, ApiKey apiKey)
    {
        await _apiKeysDao.UpdateApiKeyAsync(apiKey)
            .Match(Finish, output.Conflict);

        void Finish(Guid apiKeyId)
        {
            output.Updated(apiKeyId);
        }
    }

    private async Task<Maybe<DateTime>> GetExpirationDateAsync(IUpdateApiKeyOutputPort output, ApiKey existingApiKey,
        DateTime? expiresAt)
    {
        if (!expiresAt.HasValue)
            return Maybe<DateTime>.From(existingApiKey.ExpiresAt);

        if (expiresAt.Value < DateTime.UtcNow)
        {
            output.ExpirationDateInThePast(expiresAt.Value);
            return Maybe<DateTime>.None;
        }

        var expiresIn = expiresAt.Value - DateTime.UtcNow;

        if (expiresIn < AddApiKeyInteractor.MinExpiration)
        {
            output.ExpirationTooShort(expiresIn, AddApiKeyInteractor.MinExpiration);
            return Maybe<DateTime>.None;
        }

        if (expiresIn > AddApiKeyInteractor.MaxExpiration)
        {
            output.ExpirationTooLong(expiresIn, AddApiKeyInteractor.MaxExpiration);
            return Maybe<DateTime>.None;
        }

        var currentUser = await _userDao.GetUserAsync(existingApiKey.UserId);
        var expiresAtUtc = expiresAt.Value.Kind != DateTimeKind.Utc 
            ? expiresAt.Value.ToUtc(currentUser.TimeZone) 
            : expiresAt.Value;
        return Maybe<DateTime>.From(expiresAtUtc);
    }
}