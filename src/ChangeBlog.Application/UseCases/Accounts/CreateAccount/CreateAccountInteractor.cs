using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Application.Boundaries.DataAccess.Users;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Accounts.CreateAccount;

public class CreateAccountInteractor : ICreateAccount
{
    public const ushort MaxAccountsCreatedByUser = 5;
    private readonly IAccountDao _accountDao;
    private readonly IRolesDao _rolesDao;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserDao _userDao;

    public CreateAccountInteractor(IAccountDao accountDao, IUserDao userDao, IRolesDao rolesDao, IUnitOfWork unitOfWork)
    {
        _accountDao = accountDao;
        _rolesDao = rolesDao;
        _userDao = userDao;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(ICreateAccountOutputPort output, CreateAccountRequestModel requestModel)
    {
        if (!Name.TryParse(requestModel.Name, out var accountName))
        {
            output.InvalidName(requestModel.Name);
            return;
        }

        _unitOfWork.Start();

        var existingAccount = await _accountDao.FindAccountAsync(accountName);
        if (existingAccount.HasValue)
        {
            output.AccountAlreadyExists(existingAccount.Value.Id);
            return;
        }

        await AddAccountInternal(output, requestModel.UserId, accountName);
    }

    private async Task AddAccountInternal(ICreateAccountOutputPort output,
        Guid userId,
        Name accountName)
    {
        var accountsCreatedByCurrentUser = await _accountDao.FindByCreator(userId);

        if (accountsCreatedByCurrentUser.Count >= MaxAccountsCreatedByUser)
        {
            output.TooManyAccountsCreated(MaxAccountsCreatedByUser);
            return;
        }

        var currentUser = await _userDao.GetUserAsync(userId);
        var accountRoles = await GetAccountRolesAsync();

        var newAccount = new Account(Guid.NewGuid(), accountName, null, DateTime.UtcNow, currentUser.Id, null);
        var accountUser = new AccountUser(newAccount, currentUser, accountRoles, DateTime.UtcNow);

        await _accountDao.AddAccount(newAccount, accountUser)
            .Match(Finish, output.Conflict);

        void Finish(Guid accountId)
        {
            _unitOfWork.Commit();
            output.Created(accountId);
        }
    }

    private async Task<Role[]> GetAccountRolesAsync()
    {
        var roles = await _rolesDao.GetRolesAsync();

        var defaultUserRole = roles.SingleOrDefault(x => x.Name == Role.DefaultUser);
        if (defaultUserRole is null)
            throw new InvalidOperationException($"'{Role.DefaultUser}' role not found");

        // creator is platform manager by default
        var platformManagerRole = roles.SingleOrDefault(x => x.Name == Role.PlatformManager);
        if (platformManagerRole is null)
            throw new InvalidOperationException($"'{Role.PlatformManager}' role not found");

        return new[] {defaultUserRole, platformManagerRole};
    }
}