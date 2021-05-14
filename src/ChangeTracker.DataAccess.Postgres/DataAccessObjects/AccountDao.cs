using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
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
    }
}