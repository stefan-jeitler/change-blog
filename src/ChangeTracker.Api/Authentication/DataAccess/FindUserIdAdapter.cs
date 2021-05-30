using System;
using System.Threading.Tasks;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects.Users;

namespace ChangeTracker.Api.Authentication.DataAccess
{
    public class FindUserIdAdapter : IFindUserId
    {
        private readonly UserAccessDao _userAccessDao;

        public FindUserIdAdapter(UserAccessDao userAccessDao)
        {
            _userAccessDao = userAccessDao;
        }

        public Task<Guid?> FindAsync(string apiKey) => _userAccessDao.FindUserIdAsync(apiKey);
    }
}