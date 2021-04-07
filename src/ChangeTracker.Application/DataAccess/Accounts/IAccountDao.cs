using System;
using System.Threading.Tasks;
using ChangeTracker.Domain;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Accounts
{
    public interface IAccountDao
    {
        Task<Maybe<Account>> FindAsync(Guid accountId);
    }
}