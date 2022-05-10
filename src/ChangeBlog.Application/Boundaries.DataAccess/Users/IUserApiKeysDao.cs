using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Application.Models;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Boundaries.DataAccess.Users;

public interface IUserApiKeysDao
{
    Task<IList<UserApiKey>> GetUserApiKeysAsync(Guid userId);
    Task<Result<Guid, Conflict>> AddAsync(UserApiKey userApiKey);
}