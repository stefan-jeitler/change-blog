using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetUsers
{
    public interface IGetUsers
    {
        Task<IList<UserResponseModel>> ExecuteAsync(UsersQueryRequestModel queryRequestModel);
        Task<UserResponseModel> ExecuteAsync(Guid userId);
    }
}