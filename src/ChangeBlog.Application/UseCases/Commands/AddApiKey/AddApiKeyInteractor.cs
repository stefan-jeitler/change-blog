using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Models;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Commands.AddApiKey;

public class AddApiKeyInteractor : IAddApiKey
{
    private static readonly TimeSpan MaxExpiration = TimeSpan.FromDays(731);
    private static readonly TimeSpan MinExpiration = TimeSpan.FromDays(7);
    
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
        
        var currentUser = await _userDao.GetUserAsync(requestModel.UserId);

        if (requestModel.ExpiresIn < MinExpiration)
        {
            output.ExpirationTooShort(currentUser.Culture, requestModel.ExpiresIn, MinExpiration);
            return;
        }

        if (requestModel.ExpiresIn > MaxExpiration)
        {
            output.ExpirationTooLong(currentUser.Culture, requestModel.ExpiresIn, MaxExpiration);
            return;
        }

        if (!Name.TryParse(requestModel.Title, out var title))
        {
            output.InvalidTitle(requestModel.Title);
            return;
        }

        var expiresAt = DateTime.UtcNow + requestModel.ExpiresIn;
        var apiKey = GenerateUniqueApiKey();

        var userApiKey = new ApiKey(currentUser.Id, Guid.NewGuid(), title, apiKey, expiresAt);

        await _apiKeysDao.AddAsync(userApiKey)
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