﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Domain;
using CSharpFunctionalExtensions;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
{
    public class AccountDao : IAccountDao
    {
        private readonly IDbAccessor _dbAccessor;

        public AccountDao(IDbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task<Maybe<Account>> FindAccountAsync(Guid accountId)
        {
            const string findAccountSql = @"
                SELECT id,
                       name,
                       default_versioning_scheme_id AS defaultVersioningSchemeId,
                       created_at AS createdAt,
                       deleted_at AS deletedAt
                FROM account
                WHERE id = @accountId";

            var dbConnection = _dbAccessor.DbConnection;
            var account = await dbConnection.QuerySingleOrDefaultAsync<Account>(findAccountSql, new
            {
                accountId
            });

            return account == default
                ? Maybe<Account>.None
                : Maybe<Account>.From(account);
        }

        public async Task<Account> GetAccountAsync(Guid accountId)
        {
            var account = await FindAccountAsync(accountId);

            if (account.HasNoValue)
                throw new Exception(
                    "The requested account does not exist. If you are not sure whether the account exists use 'FindAccount' otherwise file an issue.");

            return account.Value;
        }

        public async Task<IList<Account>> GetAccountsAsync(Guid userId)
        {
            const string getAllUserAccountsSql = @"
                SELECT DISTINCT a.id,
                                a.name,
                                a.default_versioning_scheme_id AS defaultVersioningSchemeId,
                                a.created_at                   AS createdAt,
                                a.deleted_at                   AS deletedAt
                FROM account a
                         JOIN account_user au on a.id = au.account_id
                         JOIN role r on au.role_id = r.id
                         JOIN role_permission rp on r.id = rp.role_id
                WHERE au.user_id = @userId
                AND rp.permission = @permission";

            var accounts = await _dbAccessor.DbConnection
                .QueryAsync<Account>(getAllUserAccountsSql, new
                {
                    userId,
                    permission = Permission.ViewAccount.ToString()
                });

            return accounts.ToList();
        }

        public async Task<IList<Account>> GetAccountsAsync(IList<Guid> accountIds)
        {
            const string getAccountsSql = @"
                select a.id,
                       a.name,
                       a.default_versioning_scheme_id AS defaultVersioningSchemeId,
                       a.created_at                   AS createdAt,
                       a.deleted_at                   AS deletedAt
                from account a
                where a.id = ANY (@accountIds)";

            var accounts = await _dbAccessor.DbConnection
                .QueryAsync<Account>(getAccountsSql, new {accountIds});

            return accounts.ToList();
        }
    }
}