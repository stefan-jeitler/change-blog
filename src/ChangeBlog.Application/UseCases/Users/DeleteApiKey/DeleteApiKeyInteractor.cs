using System;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using ChangeBlog.Application.Boundaries.DataAccess.Users;

namespace ChangeBlog.Application.UseCases.Users.DeleteApiKey;

public class DeleteApiKeyInteractor : IDeleteApiKey
{
    private readonly IApiKeysDao _apiKeysDao;

    public DeleteApiKeyInteractor(IApiKeysDao apiKeysDao)
    {
        _apiKeysDao = apiKeysDao ?? throw new ArgumentNullException(nameof(apiKeysDao));
    }

    public Task ExecuteAsync(Guid userId, Guid apiKeyId)
    {
        Guard.Against.NullOrEmpty(userId, nameof(userId));

        return _apiKeysDao.DeleteApiKeyAsync(userId, apiKeyId);
    }
}