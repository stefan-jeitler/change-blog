using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.Models;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;

namespace ChangeBlog.Application.UseCases.Users.AddApiKey;

[UsedImplicitly]
public class AddApiKeyInteractor : IAddApiKey
{
    private const int ApiKeyLength = 26;
    private const ushort MaxApiKeys = 5;
    public static readonly TimeSpan MaxExpiration = TimeSpan.FromDays(731);
    public static readonly TimeSpan MinExpiration = TimeSpan.FromDays(7);
    private readonly IApiKeysDao _apiKeysDao;

    private readonly IUserDao _userDao;

    public AddApiKeyInteractor(IUserDao userDao, IApiKeysDao apiKeysDao)
    {
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        _apiKeysDao = apiKeysDao ?? throw new ArgumentNullException(nameof(apiKeysDao));
    }

    public async Task ExecuteAsync(IAddApiKeyOutputPort output, AddApiKeyRequestModel requestModel)
    {
        if (requestModel.UserId == Guid.Empty)
            throw new ArgumentException("userId must not be empty.");

        if (requestModel.ExpiresAt < DateTime.UtcNow)
            output.ExpirationDateInThePast(requestModel.ExpiresAt);

        var currentUser = await _userDao.GetUserAsync(requestModel.UserId);
        var expiresAt = requestModel.ExpiresAt.Kind != DateTimeKind.Utc
            ? requestModel.ExpiresAt.ToUtc(currentUser.TimeZone).ToLocal(currentUser.TimeZone)
            : requestModel.ExpiresAt.ToLocal(currentUser.TimeZone);

        var expiresIn = expiresAt.UtcDateTime - DateTime.UtcNow;
        if (expiresIn < MinExpiration)
        {
            output.ExpirationTooShort(expiresAt, MinExpiration);
            return;
        }

        if (expiresIn > MaxExpiration)
        {
            output.ExpirationTooLong(expiresAt, MaxExpiration);
            return;
        }

        var name = OptionalName.Parse(requestModel.Name);

        var apiKey = new ApiKey(currentUser.Id, Guid.NewGuid(), name, GenerateApiKey(), expiresAt.UtcDateTime);
        await AddNewApiKeyAsync(output, apiKey);
    }

    private async Task AddNewApiKeyAsync(IAddApiKeyOutputPort output, ApiKey apiKey)
    {
        var existingApiKeysCount = await _apiKeysDao.GetApiKeysCountAsync(apiKey.UserId);

        if (existingApiKeysCount >= MaxApiKeys)
        {
            output.MaxApiKeyCountReached(MaxApiKeys);
            return;
        }

        await _apiKeysDao.AddAsync(apiKey)
            .Match(Finish, output.Conflict);

        void Finish(Guid apiKeyId)
        {
            output.ApiKeyAdded(apiKeyId);
        }
    }

    private static string GenerateApiKey()
    {
        var key = new byte[ApiKeyLength];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(key);

        return Convert.ToBase64String(key);
    }
}