using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Extensions;

namespace ChangeBlog.Application.UseCases.Users.GetUsers;

public class GetUsersInteractor : IGetUsers
{
    private readonly IUserDao _userDao;

    public GetUsersInteractor(IUserDao userDao)
    {
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
    }

    public async Task<IList<UserResponseModel>> ExecuteAsync(UsersQueryRequestModel queryRequestModel)
    {
        var currentUser = await _userDao.GetUserAsync(queryRequestModel.UserId);

        var users = await _userDao.GetUsersAsync(queryRequestModel.AccountId,
            queryRequestModel.Limit,
            queryRequestModel.LastUserId,
            queryRequestModel.SearchTerm);

        return users.Select(x => new UserResponseModel(x.Id,
                x.Email,
                x.FirstName,
                x.LastName,
                x.TimeZone,
                x.Culture,
                x.CreatedAt.ToLocal(currentUser.TimeZone)))
            .ToList();
    }

    public async Task<UserResponseModel> ExecuteAsync(Guid userId)
    {
        var user = await _userDao.GetUserAsync(userId);

        return new UserResponseModel(user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            user.TimeZone,
            user.Culture,
            user.CreatedAt.ToLocal(user.TimeZone));
    }
}