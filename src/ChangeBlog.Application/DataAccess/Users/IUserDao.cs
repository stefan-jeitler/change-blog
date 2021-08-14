using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain;

namespace ChangeBlog.Application.DataAccess.Users
{
    public interface IUserDao
    {
        Task<User> GetUserAsync(Guid userId);
        Task<IList<User>> GetUsersAsync(IList<Guid> userIds);
        Task<IList<User>> GetUsersAsync(Guid accountId, ushort limit, Guid? lastUserId);
    }
}
