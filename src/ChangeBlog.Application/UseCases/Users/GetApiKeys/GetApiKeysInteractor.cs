using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;

namespace ChangeBlog.Application.UseCases.Users.GetApiKeys;

public class GetApiKeysInteractor : IGetApiKeys
{
    private readonly IUserDao _userDao;
    private readonly IApiKeysDao _apiKeysDao;

    public GetApiKeysInteractor(IUserDao userDao, IApiKeysDao apiKeysDao)
    {
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        _apiKeysDao = apiKeysDao;
    }

    public async Task<IList<ApiKeyResponseModel>> ExecuteAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("userId must not be empty.");

        var currentUser = await _userDao.GetUserAsync(userId);
        var apiKeys = await _apiKeysDao.GetApiKeysAsync(userId);

        return apiKeys
            .Select(x => new ApiKeyResponseModel(x.ApiKeyId,
                x.Title,
                x.Key,
                DateTimeExtensions.ToLocal(x.ExpiresAt, currentUser.TimeZone)))
            .ToList();
    }
}