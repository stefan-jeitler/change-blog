using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Boundaries.DataAccess.Users;

public interface IUserDao
{
    Task<User> GetUserAsync(Guid userId);
    Task<IList<User>> GetUsersAsync(IList<Guid> userIds);
    Task<IList<User>> GetUsersAsync(Guid accountId, ushort limit, Guid? lastUserId);

    Task<Maybe<User>> FindByExternalUserIdAsync(string externalUserId);

    Task<Result> AddAsync(User user);
    Task<Result> AddExternalIdentity(Models.ExternalIdentity externalIdentity);

    Task<Maybe<User>> FindByEmailAsync(string email);
}