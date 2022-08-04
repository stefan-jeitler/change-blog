using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Users.GetUsers;

public interface IGetUsers
{
    Task<IList<UserResponseModel>> ExecuteAsync(UsersQueryRequestModel queryRequestModel);
    Task<UserResponseModel> ExecuteAsync(Guid userId);
}