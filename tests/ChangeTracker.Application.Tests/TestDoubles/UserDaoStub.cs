using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Domain;

namespace ChangeTracker.Application.Tests.TestDoubles
{
    public class UserDaoStub : IUserDao
    {
        public List<User> Users { get; } = new();

        public async Task<User> GetUserAsync(Guid userId)
        {
            await Task.Yield();

            return Users.Single(x => x.Id == userId);
        }

        public async Task<IList<User>> GetUsersAsync(IList<Guid> userIds)
        {
            await Task.Yield();

            return Users
                .Where(x => userIds.Any(y => y == x.Id))
                .ToList();
        }

        /// <summary>
        /// Not properly implemented, but should be enough for the use-case tests
        /// The actual implementation of IUserDao is tested separately.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="count"></param>
        /// <param name="lastUserId"></param>
        /// <returns></returns>
        public async Task<IList<User>> GetUsersAsync(Guid accountId, ushort count, Guid? lastUserId)
        {
            await Task.Yield();

            return Users;
        }
    }
}
