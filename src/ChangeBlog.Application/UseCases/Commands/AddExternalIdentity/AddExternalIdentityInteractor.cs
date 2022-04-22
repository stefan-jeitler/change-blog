using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Application.Models;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Commands.AddExternalIdentity;

public class AddExternalIdentityInteractor : IAddExternalIdentity
{
    private readonly IExternalUserInfoDao _externalUserInfoDao;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserDao _userDao;

    public AddExternalIdentityInteractor(IExternalUserInfoDao externalUserInfoDao, IUserDao userDao,
        IUnitOfWork unitOfWork)
    {
        _externalUserInfoDao = externalUserInfoDao ?? throw new ArgumentNullException(nameof(externalUserInfoDao));
        _userDao = userDao ?? throw new ArgumentNullException(nameof(userDao));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<Guid>> ExecuteAsync(string externalUserId)
    {
        if (string.IsNullOrEmpty(externalUserId))
        {
            return Result.Failure<Guid>("ExternalUserId is null or empty.");
        }

        var userByExternalId = await _userDao.FindByExternalUserIdAsync(externalUserId);

        switch (userByExternalId.HasValue)
        {
            case true when userByExternalId.GetValueOrThrow().DeletedAt.HasValue:
                return Result.Failure<Guid>("User has been set to deleted in the App.");
            case true:
                return Result.Success(userByExternalId.GetValueOrThrow().Id);
        }

        var externalUserInfo = _externalUserInfoDao.GetUserInfo();

        return await ImportUserAsync(externalUserId, externalUserInfo);
    }

    private async Task<Result<Guid>> ImportUserAsync(string externalUserId, ExternalUserInfo externalExternalUserInfo)
    {
        _unitOfWork.Start();

        var userByEmail = await _userDao.FindByEmailAsync(externalExternalUserInfo.Email);
        var userFoundByEmail = userByEmail.HasValue;

        var user = userByEmail.HasValue
            ? userByEmail.GetValueOrThrow()
            : new User(Guid.NewGuid(), Email.Parse(externalExternalUserInfo.Email),
                Name.Parse(externalExternalUserInfo.FirstName),
                Name.Parse(externalExternalUserInfo.LastName), Default.Timezone, Default.Culture, null, DateTime.UtcNow);

        if (user.DeletedAt.HasValue)
        {
            return Result.Failure<Guid>("The user already exists, but it has been set to deleted.");
        }

        if (!userFoundByEmail)
        {
            await _userDao.AddAsync(user);
        }

        var externalIdentity = new ExternalIdentity(Guid.NewGuid(), user.Id, externalUserId,
            DateTime.UtcNow);

        var (isSuccess, _, error) = await _userDao.AddExternalIdentity(externalIdentity);

        if (!isSuccess)
        {
            return Result.Failure<Guid>($"Error while importing the user into the database. {error}");
        }

        _unitOfWork.Commit();
        return Result.Success(user.Id);
    }
}