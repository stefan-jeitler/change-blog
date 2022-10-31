using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Boundaries.DataAccess.Accounts;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Authorization;
using ChangeBlog.Domain.Miscellaneous;
using CSharpFunctionalExtensions;
using Dapper;

namespace ChangeBlog.DataAccess.Postgres.DataAccessObjects.Account;

public class AccountDao : IAccountDao
{
    private readonly IDbAccessor _dbAccessor;

    public AccountDao(IDbAccessor dbAccessor)
    {
        _dbAccessor = dbAccessor;
    }

    public async Task<Maybe<Domain.Account>> FindAccountAsync(Guid accountId)
    {
        const string findAccountSql = @"
                SELECT id,
                       name,
                       default_versioning_scheme_id AS defaultVersioningSchemeId,
                       created_at AS createdAt,
                       created_by_user AS createdByUser,
                       deleted_at AS deletedAt
                FROM account
                WHERE id = @accountId
                and deleted_at is null ";

        var dbConnection = _dbAccessor.DbConnection;
        var account = await dbConnection.QuerySingleOrDefaultAsync<Domain.Account>(findAccountSql, new
        {
            accountId
        });

        return account == default
            ? Maybe<Domain.Account>.None
            : Maybe<Domain.Account>.From(account);
    }

    public async Task<Maybe<Domain.Account>> FindAccountAsync(Name accountName)
    {
        ArgumentNullException.ThrowIfNull(accountName);

        const string findAccountSql = @"
                SELECT id,
                       name,
                       default_versioning_scheme_id AS defaultVersioningSchemeId,
                       created_at AS createdAt,
                       created_by_user AS createdByUser,
                       deleted_at AS deletedAt
                FROM account
                WHERE Lower(name) = Lower(@accountName)
                and deleted_at is null";

        var dbConnection = _dbAccessor.DbConnection;
        var account = await dbConnection.QuerySingleOrDefaultAsync<Domain.Account>(findAccountSql, new
        {
            accountName = accountName.Value
        });

        return account == default
            ? Maybe<Domain.Account>.None
            : Maybe<Domain.Account>.From(account);
    }

    public async Task<IList<Domain.Account>> FindByCreator(Guid creatorId)
    {
        const string findAccountsSql = @"
                SELECT id,
                       name,
                       default_versioning_scheme_id AS defaultVersioningSchemeId,
                       created_at AS createdAt,
                       created_by_user AS createdByUser,
                       deleted_at AS deletedAt
                FROM account
                WHERE created_by_user = @creatorId
                and deleted_at is null";

        var result = await _dbAccessor.DbConnection.QueryAsync<Domain.Account>(findAccountsSql, new {creatorId});

        return result.AsList();
    }

    public async Task<Domain.Account> GetAccountAsync(Guid accountId)
    {
        var account = await FindAccountAsync(accountId);

        if (account.HasNoValue)
            throw new Exception(
                "The requested account does not exist. If you are not sure whether the account exists use 'FindAccount' otherwise file an issue.");

        return account.GetValueOrThrow();
    }

    public async Task<IList<Domain.Account>> GetAccountsAsync(Guid userId)
    {
        const string getAllUserAccountsSql = @"
                SELECT DISTINCT a.id,
                                a.name,
                                a.default_versioning_scheme_id AS defaultVersioningSchemeId,
                                a.created_at                   AS createdAt,
                                a.created_by_user              AS createdByUser,
                                a.deleted_at                   AS deletedAt
                FROM account a
                         JOIN account_user au on a.id = au.account_id
                         JOIN role r on au.role_id = r.id
                         JOIN role_permission rp on r.id = rp.role_id
                WHERE au.user_id = @userId
                AND a.deleted_at is null
                AND rp.permission = @permission";

        var accounts = await _dbAccessor.DbConnection
            .QueryAsync<Domain.Account>(getAllUserAccountsSql, new
            {
                userId,
                permission = Permission.ViewAccount.ToString()
            });

        return accounts.AsList();
    }

    public async Task<IList<Domain.Account>> GetAccountsAsync(IList<Guid> accountIds)
    {
        const string getAccountsSql = @"
                select a.id,
                       a.name,
                       a.default_versioning_scheme_id AS defaultVersioningSchemeId,
                       a.created_at                   AS createdAt,
                       a.created_by_user              AS createdByUser,
                       a.deleted_at                   AS deletedAt
                from account a
                where a.id = ANY (@accountIds)
                and a.deleted_at is null";

        var accounts = await _dbAccessor.DbConnection
            .QueryAsync<Domain.Account>(getAccountsSql, new {accountIds});

        return accounts.AsList();
    }

    public async Task<Result<Guid, Conflict>> AddAccount(Domain.Account accountToAdd, AccountUser accountUserToAdd)
    {
        const string insertSql =
            @"insert into account (id, name, default_versioning_scheme_id, deleted_at, created_at, created_by_user)
 values (@id, @name, null, null, @createdAt, @creatorId)";

        await _dbAccessor.DbConnection.ExecuteAsync(insertSql, new
        {
            id = accountToAdd.Id,
            name = accountToAdd.Name.Value,
            createdAt = accountToAdd.CreatedAt,
            creatorId = accountToAdd.CreatedByUser
        });

        const string insertRoleSql = @"insert into account_user (account_id, user_id, role_id, created_at) 
values (@accountId, @userId, @roleId, @createdAt)";

        await _dbAccessor.DbConnection.ExecuteAsync(insertRoleSql, accountUserToAdd.Roles.Select(x =>
            new
            {
                accountId = accountUserToAdd.Account.Id,
                userId = accountUserToAdd.User.Id,
                roleId = x.Id,
                createdAt = accountUserToAdd.CreatedAt
            }));

        return Result.Success<Guid, Conflict>(accountToAdd.Id);
    }

    public async Task<Result<Guid, Conflict>> DeleteAsync(Guid accountId)
    {
        const string setDeletedAtSql = "update account set deleted_at = now() where id = @accountId";

        await _dbAccessor.DbConnection.ExecuteAsync(setDeletedAtSql, new {accountId});

        return Result.Success<Guid, Conflict>(accountId);
    }

    public async Task<Result<Guid, Conflict>> UpdateName(Guid accountId, Name newName)
    {
        ArgumentNullException.ThrowIfNull(newName);

        const string updateNameSql = "update account set name = @newName where id = @accountId";

        await _dbAccessor.DbConnection.ExecuteAsync(updateNameSql, new
        {
            newName = newName.Value,
            accountId
        });

        return Result.Success<Guid, Conflict>(accountId);
    }
}