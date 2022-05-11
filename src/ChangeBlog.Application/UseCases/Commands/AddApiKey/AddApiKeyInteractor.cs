using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Models;
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

    public async Task ExecuteAsync(IAddApiKeyOutputPort output, Guid userId, TimeSpan expiresIn)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("userId must not be empty.");
        
        var currentUser = await _userDao.GetUserAsync(userId);

        if (expiresIn < MinExpiration)
        {
            output.ExpirationTooShort(currentUser.Culture, expiresIn, MinExpiration);
            return;
        }

        if (expiresIn > MaxExpiration)
        {
            output.ExpirationTooLong(currentUser.Culture, expiresIn, MaxExpiration);
            return;
        }

        var expiresAt = DateTime.UtcNow + expiresIn;
        var apiKey = GenerateUniqueApiKey();

        var userApiKey = new UserApiKey(currentUser.Id, Guid.NewGuid(), apiKey, expiresAt);

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