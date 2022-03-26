using System;
using System.Threading.Tasks;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users;

namespace ChangeBlog.Api.Authentication;

public class FindUserId
{
    private readonly UserAccessDao _userAccessDao;

    public FindUserId(UserAccessDao userAccessDao)
    {
        _userAccessDao = userAccessDao;
    }

    public Task<Guid?> FindByApiKeyAsync(string apiKey)
    {
        return _userAccessDao.FindActiveUserIdByApiKeyAsync(apiKey);
    }

    public Task<Guid?> FindByExternalUserIdAsync(string externalUserId)
    {
        return _userAccessDao.FindActiveUserByExternalUserId(externalUserId);
    }
}