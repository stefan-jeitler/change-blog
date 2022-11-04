using System;
using System.Threading.Tasks;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users.UserAccess;

namespace ChangeBlog.Management.Api.Authentication;

public class FindUserId
{
    private readonly UserAccessDao _userAccessDao;

    public FindUserId(UserAccessDao userAccessDao)
    {
        _userAccessDao = userAccessDao;
    }

    public async Task<Guid?> FindByExternalUserIdAsync(string externalUserId)
    {
        if (string.IsNullOrEmpty(externalUserId)) return null;

        return await _userAccessDao.FindActiveUserByExternalUserId(externalUserId);
    }
}