using System;
using System.Threading.Tasks;
using ChangeTracker.DataAccess.Postgres;

namespace ChangeTracker.Api.Authentication.DataAccess
{
    public class FindUserIdAdapter : IFindUserId
    {
        private readonly UserDao _userDao;

        public FindUserIdAdapter(UserDao userDao)
        {
            _userDao = userDao;
        }

        public Task<Guid?> FindAsync(string apiKey) => _userDao.FindUserId(apiKey);
    }
}