using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;

namespace ChangeTracker.Application.UseCases.Queries.GetUsers
{
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
                queryRequestModel.LastUserId);

            return users.Select(x => new UserResponseModel(x.Id,
                    x.Email,
                    x.FirstName,
                    x.LastName,
                    x.TimeZone,
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
                user.CreatedAt.ToLocal(user.TimeZone));
        }
    }
}