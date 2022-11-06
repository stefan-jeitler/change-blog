using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Domain;
using Microsoft.Extensions.Caching.Memory;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Users.UserAccess;

public class UserAccessDaoCacheProxy : IUserAccessDao
{
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(5);
    private readonly IMemoryCache _memoryCache;
    private readonly UserAccessDao _realUserAccessDao;

    public UserAccessDaoCacheProxy(UserAccessDao realUserAccessDao, IMemoryCache memoryCache)
    {
        _realUserAccessDao = realUserAccessDao;
        _memoryCache = memoryCache;
    }

    public Task<IEnumerable<Role>> GetAccountRolesAsync(Guid userId, Guid accountId)
    {
        var cacheKey = $"AccountRoles:{userId}:{accountId}";
        return _memoryCache.GetOrCreateAsync(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
            return _realUserAccessDao.GetAccountRolesAsync(userId, accountId);
        });
    }

    public Task<AccountProductRolesDto> GetRolesByProductIdAsync(Guid userId, Guid productId)
    {
        var cacheKey = $"RolesByProductId:{userId}:{productId}";
        return _memoryCache.GetOrCreateAsync(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
            return _realUserAccessDao.GetRolesByProductIdAsync(userId, productId);
        });
    }

    public Task<AccountProductRolesDto> GetRolesByVersionIdAsync(Guid userId, Guid versionId)
    {
        var cacheKey = $"RolesByVersionId:{userId}:{versionId}";
        return _memoryCache.GetOrCreateAsync(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
            return _realUserAccessDao.GetRolesByVersionIdAsync(userId, versionId);
        });
    }

    public Task<AccountProductRolesDto> GetRolesByChangeLogLineIdAsync(Guid userId, Guid changeLogLineId)
    {
        var cacheKey = $"RolesByChangeLogLineId:{userId}:{changeLogLineId}";
        return _memoryCache.GetOrCreateAsync(cacheKey, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheExpiration;
            return _realUserAccessDao.GetRolesByChangeLogLineIdAsync(userId, changeLogLineId);
        });
    }
}