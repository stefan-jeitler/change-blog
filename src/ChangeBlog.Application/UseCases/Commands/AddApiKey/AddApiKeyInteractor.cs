using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;
using ChangeBlog.Application.Models;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Commands.AddApiKey;

public class AddApiKeyInteractor : IAddApiKey
{
    public const ushort MaxApiKeys = 5;
    public static readonly TimeSpan MaxExpiration = TimeSpan.FromDays(731);
    public static readonly TimeSpan MinExpiration = TimeSpan.FromDays(7);
    
    private readonly IUserDao _userDao;
    private readonly IApiKeysDao _apiKeysDao;

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
        {
            output.ExpirationDateInThePast(requestModel.ExpiresAt);
        }

        var currentUser = await _userDao.GetUserAsync(requestModel.UserId);
        var expiresAtUtc = requestModel.ExpiresAt.Kind != DateTimeKind.Utc 
            ? requestModel.ExpiresAt.ToUtc(currentUser.TimeZone) 
            : requestModel.ExpiresAt;

        var expiresIn =  expiresAtUtc - DateTime.UtcNow;
        if (expiresIn < MinExpiration)
        {
            output.ExpirationTooShort(expiresIn, MinExpiration);
            return;
        }

        if (expiresIn > MaxExpiration)
        {
            output.ExpirationTooLong(expiresIn, MaxExpiration);
            return;
        }

        if (!OptionalName.TryParse(requestModel.Title, out var title))
        {
            output.InvalidTitle(requestModel.Title);
            return;
        }

        var apiKey = new ApiKey(currentUser.Id, Guid.NewGuid(), title, GenerateUniqueApiKey(), expiresAtUtc);
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

    private static string GenerateUniqueApiKey()
    {
        var key = new byte[32];
        using var generator = RandomNumberGenerator.Create(); 
        generator.GetBytes(key);
        
        return Convert.ToBase64String(key);
    }
}